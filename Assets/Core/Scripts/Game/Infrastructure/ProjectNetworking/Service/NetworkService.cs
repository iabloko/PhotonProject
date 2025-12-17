using System;
using System.Collections.Generic;
using Core.Scripts.Game.Infrastructure.ProjectNetworking.Provider;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game.Infrastructure.ProjectNetworking.Service
{
    public sealed class NetworkService : INetworkService
    {
        public NetworkRunner RunnerInstance { get; set; }
        public Action<ShutdownReason> OnShutdown { get; set; }
        public ShutdownReason CurrentShutdownReason { get; private set; }
        public string CurrentSessionName { get; private set; }

        public event Action<IReadOnlyList<SessionInfo>> SessionsUpdated;
        public event Action<NetUiState> StateChanged;
        public event Action<string> ErrorRaised;

        private bool _isConnecting;

        private string _currentHostID;
        private string _gameURL;

        private bool _isReconnectSceneFullyLoaded;
        private bool _isReconnecting, _isNeedReconnecting;

        private readonly NetworkRunnerEventsHandler _networkEventsHandler;
        private readonly NetworkSessionHelper _networkSessionHelper;
        private readonly IAssetProvider _assetProvider;
        private readonly ZenjectNetworkObjectProvider _customObjectProvider;
        private readonly List<SessionInfo> _cachedSessions = new();


        [Inject]
        public NetworkService(
            IAssetProvider assetProvider,
            ZenjectNetworkObjectProvider objectProvider)
        {
            _assetProvider = assetProvider;
            _customObjectProvider = objectProvider;

            _networkSessionHelper = new NetworkSessionHelper(assetProvider);
            _networkEventsHandler = new NetworkRunnerEventsHandler(this);
        }

        IReadOnlyList<SessionInfo> INetworkService.GetSessionsSnapshot() => _cachedSessions;

        async UniTask INetworkService.ManualDisconnect(ShutdownReason reason)
        {
            Debug.Log("INetworkService Manual Disconnect");
            await RunnerInstance.Shutdown(destroyGameObject: true, reason, forceShutdownProcedure: true);
        }

        async UniTask INetworkService.Connect()
        {
            _networkEventsHandler.MegamodSessionListUpdated += HandleSessionListUpdated;
            await ConnectToLobby(SessionLobby.Custom, _networkSessionHelper.MegamodLobbyName);
        }

        private async UniTask<bool> ConnectToLobby(SessionLobby lobbyType, string customLobbyName = null)
        {
            if (_isConnecting) return false;

            _isConnecting = true;
            _networkSessionHelper.CreateCancellationTokenSource();
            SetState(NetUiState.ConnectingLobby);

            try
            {
                if (RunnerInstance == null) CreateNetworkRunner();

                StartGameResult joinLobbyResult = lobbyType switch
                {
                    SessionLobby.Custom => await RunnerInstance.JoinSessionLobby(SessionLobby.Custom, customLobbyName),
                    SessionLobby.Shared => await RunnerInstance.JoinSessionLobby(SessionLobby.Shared),
                    _ => await RunnerInstance.JoinSessionLobby(SessionLobby.ClientServer),
                };

                if (!joinLobbyResult.Ok)
                {
                    ErrorRaised?.Invoke($"INetworkService Failed to join lobby: {joinLobbyResult.ShutdownReason}");
                    Debug.LogError($"INetworkService Join lobby failed: {joinLobbyResult.ShutdownReason}");

                    await ShutdownAndCleanupAsync(ShutdownReason.Error, force: true);
                    Reconnect(ShutdownReason.Error);
                    return false;
                }

                SetState(NetUiState.InLobby);

                Debug.Log(
                    $"INetworkService Joined lobby {lobbyType}{(lobbyType == SessionLobby.Custom ? $":{customLobbyName}" : "")}");
                return true;
            }
            catch (OperationCanceledException oce)
            {
                Debug.LogWarning($"INetworkService ConnectToLobby canceled: {oce.Message}");
                await ShutdownAndCleanupAsync(ShutdownReason.Ok, force: true);
                return false;
            }
            catch (Exception e)
            {
                ErrorRaised?.Invoke($"INetworkService ConnectToLobby exception: {e.Message}");
                Debug.LogError($"INetworkService ConnectToLobby exception: {e}");
                SetState(NetUiState.Error);

                await ShutdownAndCleanupAsync(ShutdownReason.Error, force: true);
                Reconnect(ShutdownReason.Error);
                return false;
            }
            finally
            {
                _isConnecting = false;
            }
        }

        async UniTask<bool> INetworkService.ConnectToSession(string sessionName)
        {
            if (RunnerInstance == null) CreateNetworkRunner();

            CurrentSessionName = sessionName;
            SetState(NetUiState.ConnectingSession);

            try
            {
                StartGameArgs startArgs = _networkSessionHelper.CreateStartGameArgs(sessionName, _customObjectProvider);
                StartGameResult startResult = await RunnerInstance.StartGame(startArgs);

                if (!startResult.Ok)
                {
                    ErrorRaised?.Invoke(
                        $"INetworkService StartGame failed: {startResult.ShutdownReason}\n{startResult.ErrorMessage}");
                    SetState(NetUiState.Error);
                    Debug.LogError(
                        $"INetworkService StartGame failed: {startResult.ShutdownReason}\n{startResult.ErrorMessage}");

                    await ShutdownAndCleanupAsync(ShutdownReason.Error, force: true);
                    Reconnect(ShutdownReason.Error);
                    return false;
                }

                SetState(NetUiState.InSession);
                _networkEventsHandler.MegamodSessionListUpdated -= HandleSessionListUpdated;
                Debug.Log($"INetworkService StartGame OK. Joined session '{sessionName}'");

                return true;
            }
            catch (OperationCanceledException oce)
            {
                ErrorRaised?.Invoke($"INetworkService ConnectToSession exception: {oce.Message}");
                SetState(NetUiState.Error);

                Debug.LogWarning($"INetworkService ConnectToSession canceled: {oce.Message}");
                await ShutdownAndCleanupAsync(ShutdownReason.Ok, force: true);
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"INetworkService ConnectToSession exception: {e}");
                await ShutdownAndCleanupAsync(ShutdownReason.Error, force: true);
                Reconnect(ShutdownReason.Error);
                return false;
            }
        }

        private void CreateNetworkRunner()
        {
            RunnerInstance = _assetProvider.InstantiateObject<NetworkRunner>(
                AssetPaths.NETWORK_RUNNER, dontDestroy: true);
            RunnerInstance.ProvideInput = true;

            Debug.Log(
                $"[INetworkService] Runner ProvideInput={RunnerInstance.ProvideInput}, Mode={RunnerInstance.GameMode}, IsRunning={RunnerInstance.IsRunning}");
            _networkEventsHandler.RegisterCallbacks(RunnerInstance);
        }

        private async UniTask ShutdownAndCleanupAsync(ShutdownReason reason, bool force)
        {
            if (RunnerInstance == null) return;

            _networkEventsHandler?.UnregisterCallbacks(RunnerInstance);

            try
            {
                await RunnerInstance.Shutdown(destroyGameObject: true, reason, force);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"INetworkService Runner Shutdown exception: {e}");
            }
            finally
            {
                RunnerInstance = null;
            }
        }

        private void HandleSessionListUpdated(List<SessionInfo> sessions)
        {
            _cachedSessions.Clear();
            if (sessions != null) _cachedSessions.AddRange(sessions);
            SessionsUpdated?.Invoke(_cachedSessions);
        }

        private void Reconnect(ShutdownReason shutdownReason)
        {
            Debug.LogError($"INetworkService ERROR TO CONNECT TO ROOM / LOBBY / SESSION {shutdownReason}");
        }

        private void SetState(NetUiState s) => StateChanged?.Invoke(s);
    }
}
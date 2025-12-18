using System;
using System.Collections.Generic;
using Core.Scripts.Game.Infrastructure.ProjectNetworking.Provider;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.ScriptableObjects.Configs.Logger;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using Zenject;
using LogLevel = Core.Scripts.Game.ScriptableObjects.Configs.Logger.LogLevel;

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
        private readonly GameLogger _logger;


        [Inject]
        public NetworkService(
            IAssetProvider assetProvider,
            ZenjectNetworkObjectProvider objectProvider,
            GameLogger logger)
        {
            _logger = logger;
            _assetProvider = assetProvider;
            _customObjectProvider = objectProvider;

            _networkSessionHelper = new NetworkSessionHelper(assetProvider, logger);
            _networkEventsHandler = new NetworkRunnerEventsHandler(this, logger);
        }

        IReadOnlyList<SessionInfo> INetworkService.GetSessionsSnapshot() => _cachedSessions;

        async UniTask INetworkService.ManualDisconnect(ShutdownReason reason)
        {
            await RunnerInstance.Shutdown(destroyGameObject: true, reason, forceShutdownProcedure: true);
        }

        async UniTask INetworkService.Connect()
        {
            _networkEventsHandler.SessionListUpdated += HandleSessionListUpdated;
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
                    string message = $"INetworkService Join lobby failed: {joinLobbyResult.ShutdownReason}";
                    
                    ErrorRaised?.Invoke(message);
                    _logger.Log<NetworkService>(LogLevel.Error, message);
                    
                    await ShutdownAndCleanupAsync(ShutdownReason.Error, force: true);
                    Reconnect(ShutdownReason.Error);
                    return false;
                }

                SetState(NetUiState.InLobby);
                return true;
            }
            catch (OperationCanceledException oce)
            {
                string message = $"INetworkService ConnectToLobby canceled: {oce.Message}";
                
                _logger.Log<NetworkService>(LogLevel.Warning, message);
                
                await ShutdownAndCleanupAsync(ShutdownReason.Ok, force: true);
                return false;
            }
            catch (Exception e)
            {
                string message = ($"INetworkService ConnectToLobby exception: {e.Message}");
                
                ErrorRaised?.Invoke(message);
                _logger.Log<NetworkService>(LogLevel.Error, message);
                
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
                    string message = $"INetworkService StartGame failed: {startResult.ShutdownReason}\n{startResult.ErrorMessage}";
                
                    ErrorRaised?.Invoke(message);
                    _logger.Log<NetworkService>(LogLevel.Error, message);
                    
                    SetState(NetUiState.Error);

                    await ShutdownAndCleanupAsync(ShutdownReason.Error, force: true);
                    Reconnect(ShutdownReason.Error);
                    return false;
                }

                SetState(NetUiState.InSession);
                _networkEventsHandler.SessionListUpdated -= HandleSessionListUpdated;
                
                return true;
            }
            catch (OperationCanceledException oce)
            {
                string message = $"INetworkService ConnectToSession exception: {oce.Message}";
                
                ErrorRaised?.Invoke(message);
                _logger.Log<NetworkService>(LogLevel.Warning, message);
                
                SetState(NetUiState.Error);
                await ShutdownAndCleanupAsync(ShutdownReason.Ok, force: true);
                return false;
            }
            catch (Exception e)
            {
                string message = $"INetworkService ConnectToSession exception: {e.Message}";
                _logger.Log<NetworkService>(LogLevel.Error, message);
                
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
                string message = $"INetworkService Runner Shutdown exception: {e.Message}";
                _logger.Log<NetworkService>(LogLevel.Warning, message);
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
            string message = $"INetworkService ERROR TO CONNECT TO ROOM / LOBBY / SESSION {shutdownReason}";
            _logger.Log<NetworkService>(LogLevel.Error, message);
        }

        private void SetState(NetUiState s) => StateChanged?.Invoke(s);
    }
}
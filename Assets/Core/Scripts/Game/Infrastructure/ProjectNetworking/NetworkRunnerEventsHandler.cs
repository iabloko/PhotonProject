using System;
using System.Collections.Generic;
using Core.Scripts.Game.Infrastructure.ProjectNetworking.Service;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.ProjectNetworking
{
    public sealed class NetworkRunnerEventsHandler : INetworkRunnerCallbacks
    {
        public event Action<List<SessionInfo>> MegamodSessionListUpdated;

        private readonly INetworkService _networkService;

        public NetworkRunnerEventsHandler(INetworkService networkService) => _networkService = networkService;

        public void RegisterCallbacks(NetworkRunner runner)
        {
            Debug.Log($"INetworkService RegisterCallbacks {runner.name}");
            runner.AddCallbacks(this);
        }

        public void UnregisterCallbacks(NetworkRunner runner)
        {
            Debug.Log($"INetworkService UnregisterCallbacks {runner.name}");
            runner.RemoveCallbacks(this);
        }

        #region INetworkRunnerCallbacks IMPLEMENTATION

        void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            Debug.Log("INetworkService OnObjectExitAOI");
        }

        void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            // Debug.Log("INetworkService OnObjectEnterAOI");
        }

        void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key,
            float progress)
        {
            Debug.Log("INetworkService OnReliableDataProgress");
        }

        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
        {
            // Debug.Log("INetworkService OnInput");
        }

        void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            Debug.Log("INetworkService OnInputMissing");
        }

        void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.LogWarning($"INetworkService OnShutdown: {shutdownReason}");
            if (shutdownReason == ShutdownReason.Ok) return;
            runner.RemoveCallbacks(this);
            _networkService.OnShutdown?.Invoke(shutdownReason);
            // _networkService.Disconnect(shutdownReason);
        }

        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            Debug.LogWarning($"INetworkService OnDisconnectedFromServer {reason}");

            // GameMenuUIStateMachine.CloseOnly(false, null, false, false);
            // runner.Shutdown(true, ShutdownReason.Error, true);
        }

        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            // DebugConfig(runner);
        }

        void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
        {
            // DebugConfig(runner);
        }

        void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner,
            NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token)
        {
            // DebugConfig(runner);
        }

        void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
        {
            if (!runner.SimulationUnityScene.IsValid())
            {
                Debug.LogError($"OnSceneLoadDone Error {runner.SimulationUnityScene}");
                return;
            }

            Debug.LogWarning($"INetworkService OnSceneLoadDone {runner.SimulationUnityScene}");
        }

        void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress,
            NetConnectFailedReason reason)
        {
            DebugConfig(runner);
            // Debug.LogError($"INetworkService OnConnectFailed {reason.ToString()}");
        }

        void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            Debug.Log($"INetworkService OnUserSimulationMessage {message}");
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key,
            ArraySegment<byte> data)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            Debug.Log($"INetworkService OnSessionListUpdated");
            
            foreach (SessionInfo sessionInfo in sessionList)
            {
                Debug.Log($"INetworkService OnSessionListUpdated sessionInfo: {sessionInfo.Name}");
            }

            MegamodSessionListUpdated?.Invoke(sessionList);
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            Debug.Log($"INetworkService OnCustomAuthenticationResponse");
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            Debug.Log($"INetworkService OnHostMigration");
        }
        
        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log($"INetworkService OnSceneLoadStart {runner.SimulationUnityScene}");
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"INetworkService OnPlayerLeft {player}");
        }

        #endregion INetworkRunnerCallbacks IMPLEMENTATION

        private void DebugConfig(NetworkRunner runner)
        {
            try
            {
                Debug.Log("Config values: " +
                          "\n ReliableDataTransferModes: " + runner.Config.Network.ReliableDataTransferModes +
                          "\n SocketRecvBufferSize: " + runner.Config.Network.SocketRecvBufferSize +
                          "\n SocketSendBufferSize: " + runner.Config.Network.SocketSendBufferSize +
                          "\n ConnectAttempts: " + runner.Config.Network.ConnectAttempts +
                          "\n ConnectInterval: " + runner.Config.Network.ConnectInterval +
                          "\n ConnectionDefaultRtt: " + runner.Config.Network.ConnectionDefaultRtt +
                          "\n ConnectionPingInterval: " + runner.Config.Network.ConnectionPingInterval +
                          "\n ConnectionShutdownTime: " + runner.Config.Network.ConnectionShutdownTime +
                          "\n ConnectionTimeout: " + runner.Config.Network.ConnectionTimeout +
                          "\n GlobalsSize: " + runner.Config.Heap.GlobalsSize +
                          "\n PageCount: " + runner.Config.Heap.PageCount +
                          "\n PageShift: " + runner.Config.Heap.PageShift);
            }
            catch (Exception ex)
            {
                Debug.LogError("DebugConfig error: " + ex.Message);
            }

            try
            {
                Debug.Log("Config runner.Simulation.Config values: " +
                          "\n Simulation.TickRate: " + (runner.TickRate));
                // "\n Simulation.ServerPacketInterval: " + (runner.server) +
                // "\n Simulation.ClientPacketInterval: " + (runner.Simulation.Config.ClientPacketInterval));
            }
            catch (Exception ex)
            {
                Debug.LogError("DebugConfig AccuracyDefaults error: " + ex.Message);
            }
        }
    }
}
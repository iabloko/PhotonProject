using System;
using System.Collections.Generic;
using Core.Scripts.Game.Infrastructure.ProjectNetworking.Service;
using Core.Scripts.Game.ScriptableObjects.Configs.Logger;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using LogLevel = Core.Scripts.Game.ScriptableObjects.Configs.Logger.LogLevel;

namespace Core.Scripts.Game.Infrastructure.ProjectNetworking
{
    public sealed class NetworkRunnerEventsHandler : INetworkRunnerCallbacks
    {
        public event Action<List<SessionInfo>> SessionListUpdated;

        private readonly INetworkService _networkService;
        private readonly GameLogger _logger;

        public NetworkRunnerEventsHandler(INetworkService networkService, GameLogger logger)
        {
            _logger = logger;
            _networkService = networkService;
        }

        public void RegisterCallbacks(NetworkRunner runner) => runner.AddCallbacks(this);

        public void UnregisterCallbacks(NetworkRunner runner) => runner.RemoveCallbacks(this);

        #region INetworkRunnerCallbacks IMPLEMENTATION

        void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key,
            float progress)
        {
        }

        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            if (shutdownReason == ShutdownReason.Ok) return;
            runner.RemoveCallbacks(this);
            _networkService.OnShutdown?.Invoke(shutdownReason);
        }

        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }

        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
        }

        void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
        {
        }

        void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }

        void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
        {
            if (!runner.SimulationUnityScene.IsValid())
            {
                string error = $"OnSceneLoadDone Error - {runner.SimulationUnityScene}";
                _logger.Log<NetworkRunnerEventsHandler>(LogLevel.Error, error);
                return;
            }

            string done = $"OnSceneLoadDone - {runner.SimulationUnityScene}";
            _logger.Log<NetworkRunnerEventsHandler>(LogLevel.Info, done);
        }

        void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress,
            NetConnectFailedReason reason)
        {
            string message = GetRunnerInfo(runner);
            _logger.Log<NetworkRunnerEventsHandler>(LogLevel.Error, message);
        }
        
        void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key,
            ArraySegment<byte> data)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            SessionListUpdated?.Invoke(sessionList);
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }
        
        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }

        #endregion INetworkRunnerCallbacks IMPLEMENTATION
        
        private static string GetRunnerInfo(NetworkRunner runner)
        {
            return ("Config values: " +
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
    }
}
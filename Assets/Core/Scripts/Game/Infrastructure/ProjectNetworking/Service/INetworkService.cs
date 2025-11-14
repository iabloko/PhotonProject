using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;

namespace Core.Scripts.Game.Infrastructure.ProjectNetworking.Service
{
    public enum NetUiState
    {
        ConnectingLobby,
        InLobby,
        ConnectingSession,
        InSession,
        Disconnecting,
        Error
    }

    public interface INetworkService
    {
        public NetworkRunner RunnerInstance { get; set; }

        public Action<ShutdownReason> OnShutdown { get; set; }
        public ShutdownReason CurrentShutdownReason { get; }
        public string CurrentSessionName { get; }
        public UniTask Connect();
        public UniTask ManualDisconnect(ShutdownReason reason);
        
        public UniTask<bool> ConnectToSession(string sessionName);
        public IReadOnlyList<SessionInfo> GetSessionsSnapshot();
        
        public event Action<IReadOnlyList<SessionInfo>> SessionsUpdated;
        public event Action<NetUiState> StateChanged;
        public event Action<string> ErrorRaised;
    }
}
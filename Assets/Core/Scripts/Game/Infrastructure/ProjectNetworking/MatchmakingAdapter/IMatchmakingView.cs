using System.Collections.Generic;
using Core.Scripts.Game.Infrastructure.ProjectNetworking.Service;
using Fusion;

namespace Core.Scripts.Game.Infrastructure.ProjectNetworking.MatchmakingAdapter
{
    public interface IMatchmakingView
    {
        void ShowState(NetUiState state);
        void ShowSessions(IReadOnlyList<SessionInfo> sessions);
        void ShowError(string message);
    }
}
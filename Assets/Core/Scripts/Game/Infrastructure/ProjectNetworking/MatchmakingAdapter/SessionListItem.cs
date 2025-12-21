using System;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.Game.Infrastructure.ProjectNetworking.MatchmakingAdapter
{
    public sealed class SessionListItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _hostName;
        [SerializeField] private TMP_Text _sessionName;
        [SerializeField] private TMP_Text _playerCount;
        [SerializeField] private Button _joinButton;

        private SessionInfo _session;
        private Action<SessionInfo> _onJoin;

        private void Awake()
        {
            _joinButton.onClick.AddListener(() => _onJoin?.Invoke(_session));
        }
        
        public void Bind(SessionInfo s, Action<SessionInfo> onJoin)
        {
            _session = s;
            _onJoin = onJoin;
            transform.name = _sessionName.text = s.Name;
            _playerCount.text = $"{s.PlayerCount} / {s.MaxPlayers}";
        }
    }
}
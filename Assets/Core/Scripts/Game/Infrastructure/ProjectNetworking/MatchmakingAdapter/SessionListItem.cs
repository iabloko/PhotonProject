using System;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.Game.Infrastructure.ProjectNetworking.MatchmakingAdapter
{
    public sealed class SessionListItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text hostName;
        [SerializeField] private TMP_Text sessionName;
        [SerializeField] private TMP_Text playerCount;
        [SerializeField] private Button joinButton;

        private SessionInfo _session;
        private Action<SessionInfo> _onJoin;

        private void Awake()
        {
            joinButton.onClick.AddListener(() => _onJoin?.Invoke(_session));
        }
        
        public void Bind(SessionInfo s, Action<SessionInfo> onJoin)
        {
            _session = s;
            _onJoin = onJoin;
            transform.name = sessionName.text = s.Name;
            playerCount.text = $"{s.PlayerCount} / {s.MaxPlayers}";
        }
    }
}
using System;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.Player
{
    public abstract class PlayerNetworkBehaviour : PlayerData
    {
        [Title("Network Behaviour", subtitle: "", TitleAlignments.Right), Networked, UnitySerializeField]
        public int CurrentHealth { get; protected set; }
        [Networked, UnitySerializeField] protected internal NetworkString<_16> PlayerNickName { get; protected set; }
        [Networked, UnitySerializeField] protected internal NetworkString<_32> PlayerSkin { get; protected set; }
        [Networked, UnitySerializeField] protected internal int SkinIndex { get; protected set; }
        [Networked, UnitySerializeField] protected internal int PlayerID { get; protected set; }

        private ChangeDetector _changeDetector;

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_PlayerDeathLogic() => Debug.LogWarning("Player Death Logic");

        public override void Spawned()
        {
            base.Spawned();
            CurrentHealth = 100;
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        }

        public override void Render()
        {
            foreach (string change in
                     _changeDetector.DetectChanges(this,
                         out NetworkBehaviourBuffer previous,
                         out NetworkBehaviourBuffer current))
            {
                switch (change)
                {
                    case nameof(PlayerNickName):
                        SetUpLocalPlayerNickName();
                        break;                    
                    case nameof(CurrentHealth):
                        HealthChanged();
                        break;
                }
            }
        }

        public void ChangeHealth(int value)
        {
            if (HasStateAuthority) CurrentHealth = value;
        }

        protected void HealthChanged()
        {
            
        }

        protected void ChangePlayerNicknameVisibility(bool status) => nickNameText.gameObject.SetActive(status);

        protected void SetUpLocalPlayerNickName()
        {
            try
            {
#if UNITY_EDITOR
                string playerName = string.Concat(PlayerNickName.Value, "_", PlayerID);
#elif !UNITY_EDITOR && UNITY_WEBGL
                string playerName = string.Concat(PlayerNickName.Value);
#endif
                nickNameText.text = playerName;
                transform.name = playerName;
            }
            catch (Exception e)
            {
                Debug.LogError($"Player Room Enter Player ID {PlayerID} ERROR {e}");
                Debug.LogError($"Player Room Enter Player ID {PlayerID} ERROR {e.Message}");
            }
        }
    }
}
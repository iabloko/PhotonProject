using System;
using Core.Scripts.Game.Player.VisualData;
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
        [Networked, UnitySerializeField] protected internal PlayerVisualNetwork VisualNetwork { get; protected set; }

        private ChangeDetector _changeDetector;

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_PlayerDeathLogic() => Debug.LogWarning("Player Death Logic");

        public override void Spawned()
        {
            base.Spawned();
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            
            if (Object.HasStateAuthority)
            {
                CurrentHealth = 100;
            }
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
                    case nameof(VisualNetwork):
                        SkinChanged();
                        break;
                }
            }
        }

        private void SkinChanged()
        {
            for (int i = 0; i < playerVisualData.hair.Length; i++)
                playerVisualData.hair[i].SetActive(i == VisualNetwork.hairID);

            for (int i = 0; i < playerVisualData.heads.Length; i++)
                playerVisualData.heads[i].SetActive(i == VisualNetwork.headID);

            for (int i = 0; i < playerVisualData.eyes.Length; i++)
                playerVisualData.eyes[i].SetActive(i == VisualNetwork.eyeID);

            for (int i = 0; i < playerVisualData.mouth.Length; i++)
                playerVisualData.mouth[i].SetActive(i == VisualNetwork.mountID);

            for (int i = 0; i < playerVisualData.bodies.Length; i++)
                playerVisualData.bodies[i].SetActive(i == VisualNetwork.bodyID);
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
                string playerName = string.Concat(PlayerNickName.Value, "_", Object.Id);
#elif !UNITY_EDITOR && UNITY_WEBGL
                string playerName = string.Concat(PlayerNickName.Value);
#endif
                nickNameText.text = playerName;
                transform.name = playerName;
            }
            catch (Exception e)
            {
                Debug.LogError($"Player Room Enter Player ID {Object.Id} ERROR {e}");
                Debug.LogError($"Player Room Enter Player ID {Object.Id} ERROR {e.Message}");
            }
        }
    }
}
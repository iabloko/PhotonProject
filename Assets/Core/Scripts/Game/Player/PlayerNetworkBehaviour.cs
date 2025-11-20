using System;
using System.Threading;
using Core.Scripts.Game.ObjectDamageReceiver;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.Player
{
    public abstract class PlayerNetworkBehaviour : PlayerData
    {
        [Title("Network Behaviour", subtitle: "", TitleAlignments.Right), Networked, UnitySerializeField]
        protected internal NetworkBool SkinVisibility { get; protected set; }
        [Networked, UnitySerializeField] protected internal NetworkString<_16> PlayerNickName { get; protected set; }
        [Networked, UnitySerializeField] protected internal NetworkString<_32> PlayerSkin { get; protected set; }
        [Networked, UnitySerializeField] protected internal int SkinIndex { get; protected set; }
        [Networked, UnitySerializeField] protected internal int PlayerID { get; protected set; }
        [Networked, UnitySerializeField] protected internal int IsPlayerMoving { get; protected set; }

        [Networked, UnitySerializeField] public int CurrentHealth { get; protected set; }

        private ChangeDetector _changeDetector;

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_PlayerDeathLogic()
        {
            Debug.LogWarning("Player Death Logic");
            // playerDeathEffect.CreateDeathEffect(position, rotation, scale, isLine).Forget();
        }

        public override void Spawned()
        {
            base.Spawned();
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
                    case nameof(SkinIndex):
                        SetNewSkin();
                        break;
                    case nameof(PlayerSkin):
                        DownloadPlayerSkin();
                        break;
                    case nameof(SkinVisibility):
                        ApplyChangedVisibility();
                        break;
                    case nameof(PlayerNickName):
                        SetUpLocalPlayerNickName();
                        break;
                    case nameof(IsPlayerMoving):
                        PlayerNetworkMovementLogic();
                        break;
                }
            }
        }

        public void ChangeHealth(int value)
        {
            if (HasStateAuthority) CurrentHealth = value;
        }

        private void ChangePlayerNickName(string playerNickName)
        {
            if (HasStateAuthority) PlayerNickName = playerNickName;
        }

        private void ChangePlayerSkinId(string playerSkinId)
        {
            if (HasStateAuthority) PlayerSkin = playerSkinId;
        }

        private void PlayerNetworkMovementLogic()
        {
        }

        #region PLAYER_NICK_NAME

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

        #endregion

        #region SKIN

        private void DownloadPlayerSkin()
        {
            using CancellationTokenSource source = new();
            string skinId = PlayerSkin.Value;
            // SkinService.ApplySkinAsync(skinId, source.Token).Forget();
        }

        protected void SetEnablePlayerModel(bool enable) => SkinVisibility = enable;

        protected void ApplyChangedVisibility(int layer)
        {
            // SkinVisibilityController.SetLayer(layer);
        }

        private void ApplyChangedVisibility()
        {
            // SkinVisibilityController.SetEnabled(SkinVisibility);
        }

        protected void SetNewSkin()
        {
            // SkinVisibilityController.SetNewSkin(playerVisualData.skins[SkinIndex]);
        }

        protected void ChangePlayerNicknameVisibility(bool status)
        {
            nickNameText.gameObject.SetActive(status);

            // status = !HasStateAuthority && status;
            // int layer = LayerMask.NameToLayer(status ? "OtherPlayersNickname" : "PlayerNickname");
            // nickNameText.gameObject.layer = layer;
        }

        #endregion
    }
}
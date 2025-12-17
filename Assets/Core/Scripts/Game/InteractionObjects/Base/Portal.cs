using System;
using System.Threading;
using Core.Scripts.Game.Infrastructure.ModelData.InteractionObjects;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Scripts.Game.InteractionObjects.Base
{
    public enum PortalEffectStatus
    {
        PortalEffectOn,
        PortalEffectOff,
    }

    public enum PortalKeys
    {
        PrimaryPortal,
        SecondPortal
    }

    public abstract class Portal : InteractionObject
    {
        public string PortalKey;
        public int PortalID;

        public Portal nextPortal;

        [SerializeField] protected Transform spawnPoint;
        [SerializeField] protected ParticleSystem FXportalOn;
        [SerializeField] protected ParticleSystem FXportalOff;
        
        private CancellationToken _cancellationToken;
        
        public override void Spawned()
        {
            base.Spawned();
            _cancellationToken = this.GetCancellationTokenOnDestroy();
        }

        public abstract void UpdatePortalKey(int prefix);

        public void OnPlayerTrigger()
        {
            if (WasInteracted) return;
            WasInteracted = true;

            PrepareToTeleportation();
        }

        public override void StopInteract()
        {
            base.StopInteract();
            WasInteracted = false;
        }

        public void OnBotTrigger() => BotTriggerLogic();

        private void BotTriggerLogic() => PlayPortalEffect(PortalEffectStatus.PortalEffectOn);

        private void PrepareToTeleportation()
        {
            Teleportation().Forget();
        }

        private async UniTaskVoid Teleportation()
        {
            //Disable Player visual data and movement

            await ActivationPortalEffect();

            nextPortal.WasInteracted = true;

            await UniTask.Delay(100, cancellationToken: _cancellationToken);

            // Enable Player visual data and movement, and change position

            await DeactivationPortalEffect();

            CompleteTeleportation().Forget();
        }

        private async UniTask CompleteTeleportation()
        {
            await UniTask.Delay(50, cancellationToken: _cancellationToken);

            WasInteracted = false;
        }

        #region PORTAL_EFFECTS

        private async UniTask ActivationPortalEffect()
        {
            //TODO FIX
            // if (NetworkOnSceneManagement.LocalInstance != null)
            // {
            //     NetworkOnSceneManagement.LocalInstance.RPC_ShowPortalEffect((int)PortalEffectStatus.PortalEffectOn,
            //         PortalKey);
            // }

            while (FXportalOn.isPlaying)
                await UniTask.Yield(_cancellationToken);
        }

        private async UniTask DeactivationPortalEffect()
        {
            //TODO FIX
            // if (NetworkOnSceneManagement.LocalInstance != null)
            // {
            //     NetworkOnSceneManagement.LocalInstance.RPC_ShowPortalEffect((int)PortalEffectStatus.PortalEffectOff,
            //         nextPortal.PortalKey);
            // }


            while (FXportalOff.isPlaying)
                await UniTask.Yield(_cancellationToken);
        }

        public void ShowPortalEffect(int effectType)
        {
            if (Enum.IsDefined(typeof(PortalEffectStatus), effectType))
            {
                PortalEffectStatus effectStatus = (PortalEffectStatus)effectType;
                PlayPortalEffect(effectStatus);
            }
            else
            {
                Debug.LogError("ShowPortalEffect Enum type Error");
            }
        }

        private void PlayPortalEffect(PortalEffectStatus effectStatus)
        {
            if (effectStatus == PortalEffectStatus.PortalEffectOn)
            {
                if (FXportalOn.isPlaying) return;
                FXportalOn.Play();
            }
            else
            {
                if (FXportalOff.isPlaying) return;
                FXportalOff.Play();
            }
        }

        #endregion
    }
}
using Core.Scripts.Game.Player.PlayerEffects.SimpleEffects;
using Core.Scripts.Game.Player.PlayerMovementEffects;
using Fusion.Addons.SimpleKCC;
using Sandbox.Project.Scripts.Infrastructure.ModelData.InteractionObjects;
using Sandbox.Project.Scripts.Infrastructure.ModelData.MovementEffects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.Player.PlayerEffects
{
    [System.Serializable, HideLabel]
    public sealed class MovementEffects
    {
        // public bool isBananaEffect;
        // public bool isBubbleGumEffect;
        // public bool isSpeedBoostEffect;

        // private SpeedUpSpecialObjectEffect _speedUpSpecialObjectEffect;
        // private BubbleGumSpecialObjectEffect _bubbleGumSpecialObjectEffect;
        // private BananaSpecialObjectEffect _bananaSpecialObjectEffect;
        // private JumPadSpecialObjectEffect _jumPadSpecialObjectEffect;
        
        private IPlayerEffect _footPrintEffect;

        private readonly SimpleKCC _kcc;

        // private bool PlayParticles => !isBananaEffect && !isBubbleGumEffect;

        public MovementEffects(SimpleKCC kcc) => _kcc = kcc;

        public void CreateMovementEffects(Animator animator, ParticleSystem footprintParticles)
        {
            // _bananaSpecialObjectEffect = new BananaSpecialObjectEffect(animator, EffectDuration.BANANA_DURATION);
            // _bubbleGumSpecialObjectEffect =
            //     new BubbleGumSpecialObjectEffect(animator, EffectDuration.BUBBLEGUM_SLOWDOWN_DURATION);
            // _speedUpSpecialObjectEffect = new SpeedUpSpecialObjectEffect(EffectDuration.SPEED_UP_BOOST_DURATION);
            // _jumPadSpecialObjectEffect = new JumPadSpecialObjectEffect(animator, 0);

            _footPrintEffect = new FootPrint(footprintParticles);
        }

        public void UpdateMovementEffects()
        {
            // _bananaSpecialObjectEffect.UpdateEffectTimer();
            // _bubbleGumSpecialObjectEffect.UpdateEffectTimer();
            // _speedUpSpecialObjectEffect.UpdateEffectTimer();
            //
            // isBananaEffect = _bananaSpecialObjectEffect.IsActiveNow;
            // isBubbleGumEffect = _bubbleGumSpecialObjectEffect.IsActiveNow;
            // isSpeedBoostEffect = _speedUpSpecialObjectEffect.IsActiveNow;
        }

        public void UpdatePlayerEffects(bool isPlayerMoving)
        {
            // if (PlayParticles && isPlayerMoving)
            if (isPlayerMoving)
            {
                EffectsOnMovementLogic();
            }
            else
            {
                EffectsOnStopMovementLogic();
            }

            EffectsOnUpdateLogic();
        }

        public void StopMovementEffects()
        {
            // _bananaSpecialObjectEffect.Stop();
            // _bubbleGumSpecialObjectEffect.Stop();
            // _speedUpSpecialObjectEffect.Stop();
        }

        public void StartMovementEvent(MovementEffectData movementData)
        {
            switch (movementData.Type)
            {
                case InteractionObjectType.BANANA:
                    StartBanana();
                    break;
                case InteractionObjectType.BUBBLE_GUM:
                    StartBubbleGum();
                    break;
                case InteractionObjectType.SPEED_BOOSTER_PLANE:
                    StartSpeedUpBoost();
                    break;
                case InteractionObjectType.JUMP_PAD:
                    StartJumPad();
                    break;
            }
        }

        private void StartJumPad()
        {
            if (!_kcc.IsGrounded) return;
            // _jumPadSpecialObjectEffect.Start();
        }

        private void StartBanana()
        {
            if (!_kcc.IsGrounded) return;
            // _bananaSpecialObjectEffect.Start();
        }

        private void StartBubbleGum()
        {
            if (!_kcc.IsGrounded) return;
            // _bubbleGumSpecialObjectEffect.Start();
        }

        private void StartSpeedUpBoost()
        {
            if (!_kcc.IsGrounded) return;
            // _speedUpSpecialObjectEffect.Start();
        }

        private void EffectsOnStopMovementLogic() => _footPrintEffect.OnPlayerStop();
        private void EffectsOnMovementLogic() => _footPrintEffect.OnPlayerMovement();
        private void EffectsOnUpdateLogic() => _footPrintEffect.OnUpdateCall();
    }
}
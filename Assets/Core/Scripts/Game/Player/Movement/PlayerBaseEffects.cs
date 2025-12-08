using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.Player.Movement
{
    public abstract class PlayerBaseEffects : PlayerBaseRotation
    {
        [Title("Effects", "", TitleAlignments.Right), SerializeField]
        private ParticleSystem footprintParticles;

        [SerializeField] private ParticleSystem onGroundParticles;
        [SerializeField] private Effects.MovementEffects movementEffects;

        private PlayerTeleportationData _playerTeleportationData;

        private bool _wasGroundedLastTick;

        private const float ON_GROUND_MIN_THRESHOLD = -20f;

        public override void Spawned()
        {
            movementEffects = new Effects.MovementEffects(kcc);
            movementEffects.CreateMovementEffects(animator, footprintParticles);

            if (!Object.HasStateAuthority) return;
            base.Spawned();
        }

        public virtual void SaveDataForKccTeleportation(PlayerTeleportationData data)
        {
            _playerTeleportationData = data;
            movementEffects.StopMovementEffects();
        }

        public override void BeforeTick()
        {
            _wasGroundedLastTick = kcc.IsGrounded;
        }

        public override void AfterTick()
        {
            if (kcc.IsGrounded && !_wasGroundedLastTick && kcc.RealVelocity.y < ON_GROUND_MIN_THRESHOLD)
            {
                OnGroundEffect();
            }
        }

        private void OnGroundEffect()
        {
            onGroundParticles.Play();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            
            bool showPlayerEffect = IsPlayerShifting && kcc.IsGrounded;
            movementEffects.UpdatePlayerEffects(showPlayerEffect);
            movementEffects.UpdateMovementEffects();
        }

        protected void StartTeleportation()
        {
            kcc.ResetVelocity();
            kcc.SetPosition(_playerTeleportationData.endPosition, teleport: true, allowAntiJitter: false);
            kcc.SetLookRotation(_playerTeleportationData.endRotation);
        }
    }
}
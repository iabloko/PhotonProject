using Core.Scripts.Game.Player.PlayerEffects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.Player.Locomotion
{
    public abstract class PlayerBaseEffects : PlayerBaseRotation
    {
        [Title("Effects", "", TitleAlignments.Right), SerializeField]
        private ParticleSystem footprintParticles;
        [SerializeField] private ParticleSystem onGroundParticles;

        [SerializeField] private MovementEffects movementEffects;

        private PlayerTeleportationData _playerTeleportationData;

        public override void Spawned()
        {
            movementEffects = new MovementEffects(kcc);
            movementEffects.CreateMovementEffects(animator, footprintParticles);

            if (!Object.HasStateAuthority) return;
            base.Spawned();
        }
        
        public virtual void SaveDataForKccTeleportation(PlayerTeleportationData data)
        {
            _playerTeleportationData = data;
            movementEffects.StopMovementEffects();
        }

        protected void OnGroundEffect()
        {
            onGroundParticles.Play();
        }

        protected override void LateUpdate()
        {
            UpdatePlayerEffects();
            movementEffects.UpdateMovementEffects();
            
            if (!Object.HasStateAuthority) return;
            base.LateUpdate();
        }

        #region MOVEMENT EFFECTS

        private void UpdatePlayerEffects()
        {
            movementEffects.UpdatePlayerEffects(IsPlayerMoving);
        }

        protected void StartTeleportation()
        {
            kcc.ResetVelocity();
            kcc.SetPosition(_playerTeleportationData.endPosition, teleport: true, allowAntiJitter: false);
            kcc.SetLookRotation(_playerTeleportationData.endRotation);
        }

        #endregion
    }
}
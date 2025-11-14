using Core.Scripts.Game.Player.PlayerEffects;
using Sandbox.Project.Scripts.Infrastructure.ModelData.InteractionObjects;
using Sandbox.Project.Scripts.Infrastructure.ModelData.MovementEffects;
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

        // protected bool IsBounced;
        // protected bool IsBananaEffect => movementEffects.isBananaEffect;
        // protected bool IsBubbleGumEffect => movementEffects.isBubbleGumEffect;
        // protected bool IsSpeedBoostEffect => movementEffects.isSpeedBoostEffect;

        // protected float JumpPadImpulse;
        // protected Vector3 JumpPadForwardDirection;
        // protected Vector3 JumpPadVelocity;

        private PlayerTeleportationData _playerTeleportationData;

        public override void Spawned()
        {
            movementEffects = new MovementEffects(kcc);
            movementEffects.CreateMovementEffects(animator, footprintParticles);

            if (!Object.HasStateAuthority) return;
            base.Spawned();
        }

        public virtual void StartMovementEvent(MovementEffectData movementData) =>
            movementEffects.StartMovementEvent(movementData);

        public virtual void SaveDataForKccTeleportation(PlayerTeleportationData data)
        {
            _playerTeleportationData = data;
            movementEffects.StopMovementEffects();
        }

        protected void OnGroundEffect()
        {
            // if (ProjectSettings.IsGamePaused) return;
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
            // if (!PlayerInfo.PlayerInStatus.IsPlayerTeleportation) return;

            kcc.ResetVelocity();
            kcc.SetPosition(_playerTeleportationData.endPosition, teleport: true, allowAntiJitter: false);
            kcc.SetLookRotation(_playerTeleportationData.endRotation);
        }

        protected void TryToCompleteTeleportation()
        {
            // if (PlayerInfo.PlayerInStatus.IsPlayerTeleportation)
            // {
            //     PlayerInfo.PlayerInStatus.ChangePlayerEnterPortalStatus(false);
            //     PlayerInfo.PlayerInStatus.ChangePlayerTeleportationStatus(false);
            // }
        }

        #endregion
    }
}
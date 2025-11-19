using Core.Scripts.Game.Infrastructure.ModelData;
using Core.Scripts.Game.Infrastructure.Services.CinemachineService;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using Core.Scripts.Game.Player.NetworkInput;
using Fusion;
using Fusion.Addons.SimpleKCC;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game.Player.Locomotion
{
    public abstract class PlayerBaseData : NetworkBehaviour, IAfterSpawned, IBeforeTick, IAfterTick
    {
        [Title("Base Data", "", TitleAlignments.Right), SerializeField]
        protected SimpleKCC kcc;
        [SerializeField] protected PlayerInput input;

        [InfoBox("The data will be filled in at the beginning of the game " +
                 "when we receive information about the room settings."), SerializeField]
        protected RoomSettings roomData;

        protected bool IsPlayerShifting;
        protected bool IsPlayerMoving =>
            (Object != null && !Object.HasStateAuthority ? IsMovingByKcc() : IsMovingByInput()) && kcc.IsGrounded;
        protected bool IsPlayerMovingOrFalling =>
            (Object != null && !Object.HasStateAuthority ? IsMovingByKcc() : IsMovingByInput()) || !kcc.IsGrounded;

        #region SERVECES

        protected ICinemachine Cinemachine;
        protected IProjectSettings ProjectSettings;

        #endregion

        [Inject]
        public void Constructor(ICinemachine cinemachine, IProjectSettings projectSettings)
        {
            Cinemachine = cinemachine;
            ProjectSettings = projectSettings;
        }

        public abstract void AfterSpawned();
        public abstract void AfterTick();
        public abstract void BeforeTick();

        private bool IsMovingByInput() =>
            roomData.settings.autoRun ||
            !Mathf.Approximately(input.CurrentInput.MoveDirection.x, 0) ||
            !Mathf.Approximately(input.CurrentInput.MoveDirection.y, 0);

        private bool IsMovingByKcc()
        {
            Vector3 v = kcc.RealVelocity;
            v.y = 0f;

            return v.sqrMagnitude > 0.01f;
        }
    }
}
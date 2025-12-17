using Core.Scripts.Game.Infrastructure.ModelData;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using Core.Scripts.Game.PlayerLogic.NetworkInput;
using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;

namespace Core.Scripts.Game.PlayerLogic.ContextLogic
{
    public sealed class PlayerContext : Context
    {
        public PlayerInput Input { get; }
        
        public bool IsMoving =>
            (!Authority ? IsMovingByKcc() : IsMovingByInput()) && Kcc.IsGrounded;
        
        public bool IsMovingOrFalling =>
            (!Authority ? IsMovingByKcc() : IsMovingByInput()) || !Kcc.IsGrounded;

        public bool IsPlayerShifting => Shifting();
        
        public PlayerContext(
            SimpleKCC kcc,
            PlayerInput input,
            RoomSettings roomData,
            IProjectSettings projectSettings,
            NetworkRunner runner,
            bool authority) : base(authority, kcc, runner, roomData, projectSettings) => Input = input;

        private bool IsMovingByInput() =>
            RoomData.settings.autoRun ||
            !Mathf.Approximately(Input.CurrentInput.MoveDirection.x, 0) ||
            !Mathf.Approximately(Input.CurrentInput.MoveDirection.y, 0);

        private bool IsMovingByKcc()
        {
            Vector3 v = Kcc.RealVelocity;
            v.y = 0f;

            return v.sqrMagnitude > 0.01f;
        }

        private bool Shifting()
        {
            InputModelData curr = Input.CurrentInput;
            bool isShiftButtonPressed = curr.Actions.IsSet(InputModelData.SHIFT_BUTTON);
            return RoomData.settings.shiftMode && isShiftButtonPressed;
        }
    }
}
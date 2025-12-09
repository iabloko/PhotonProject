using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using Core.Scripts.Game.PlayerLogic.ContextLogic;
using Core.Scripts.Game.PlayerLogic.NetworkInput;
using UnityEngine;

namespace Core.Scripts.Game.PlayerLogic.Movement
{
    public sealed class Moving
    {
        private readonly PlayerContext _ctx;
        private readonly IProjectSettings _projectSettings;
        private readonly System.Action _onJumpAnimation;
        
        private Vector3 _moveVelocity;
        private Vector3 _desiredMoveDirection;
        
        private float _jumpImpulse;
        private float _acceleration;
        
        public Moving(PlayerContext ctx, IProjectSettings settings, System.Action jump)
        {
            _ctx = ctx;
            _projectSettings = settings;
            _onJumpAnimation = jump;
        }
        
        public void AfterSpawned()
        {
            _ctx.Kcc.SetMaxGroundAngle(75f);
        }
        
        public void AfterTick()
        {
            _jumpImpulse = 0;
        }  

        public void FixedUpdateNetwork()
        {
            _ctx.Kcc.SetGravity(_ctx.Kcc.RealVelocity.y >= 0 ? _ctx.RoomData.settings.upGravity : _ctx.RoomData.settings.downGravity);
            
            if (_projectSettings.IsGamePaused)
            {
                _moveVelocity = _desiredMoveDirection = Vector3.zero;
                _ctx.Kcc.ResetVelocity();
                _ctx.Kcc.Move(_ctx.Kcc.IsGrounded ? Vector3.zero : new Vector3(0, _ctx.RoomData.settings.downGravity, 0));
                return;
            }

            CalculateDesiredMoveDirection();
            CalculateJumpImpulse();
            CalculateAcceleration();
            CalculateMoveVelocity();

            _ctx.Kcc.Move(_moveVelocity, _jumpImpulse);
        }
        
        private void CalculateDesiredMoveDirection()
        {
            float currentSpeed = CalculateSpeed();

            Vector3 inputDirection = _ctx.Kcc.TransformRotation * new Vector3(
                _ctx.Input.CurrentInput.MoveDirection.x, 0.0f,
                _ctx.Input.CurrentInput.MoveDirection.y);

            if (inputDirection.sqrMagnitude > 1f)
                inputDirection.Normalize();

            _desiredMoveDirection = _ctx.RoomData.settings.autoRun ? _ctx.Kcc.TransformDirection : inputDirection;
            _desiredMoveDirection *= currentSpeed;

            if (_ctx.Kcc.ProjectOnGround(_desiredMoveDirection, projectedVector: out Vector3 moveVelocity))
            {
                _desiredMoveDirection =
                    Vector3.ClampMagnitude(moveVelocity.normalized * currentSpeed, currentSpeed);
            }
        }
        
        private void CalculateJumpImpulse()
        {
            if (!_ctx.Kcc.IsGrounded) return;

            if (_ctx.Input.CurrentInput.Actions.WasPressed(_ctx.Input.PreviousInput.Actions, InputModelData.JUMP_BUTTON))
            {
                _jumpImpulse = _ctx.RoomData.settings.localJumpForce * _ctx.RoomData.settings.jumpFactor;
                _onJumpAnimation?.Invoke();
            }
        }
        
        private void CalculateAcceleration()
        {
            if (_desiredMoveDirection == Vector3.zero)
            {
                _acceleration = _ctx.Kcc.IsGrounded
                    ? _ctx.RoomData.settings.groundDeceleration
                    : _ctx.RoomData.settings.airDeceleration;
            }
            else
            {
                _acceleration = _ctx.Kcc.IsGrounded
                    ? _ctx.RoomData.settings.groundAcceleration
                    : _ctx.RoomData.settings.airAcceleration;
            }
        }

        private void CalculateMoveVelocity() => 
            _moveVelocity = Vector3.Lerp(_moveVelocity, _desiredMoveDirection, _acceleration * _ctx.Runner.DeltaTime);

        private float CalculateSpeed()
        {
            float currentSpeed = _ctx.IsPlayerShifting ? _ctx.RoomData.settings.runningSpeed : _ctx.RoomData.settings.walkingSpeed;
            return currentSpeed;
        }
    }
}
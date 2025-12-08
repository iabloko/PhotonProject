using Core.Scripts.Game.Player.NetworkInput;
using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.Player.Movement
{
    public sealed class PlayerBaseMovement : PlayerBaseEffects
    {
        [Networked] private Vector3 MoveVelocity { get; set; }

        private Vector3 _desiredMoveDirection;

        private float _jumpImpulse;
        private float _acceleration;

        public override void Spawned()
        {
            base.Spawned();

            if (!Object.HasStateAuthority) return;
            kcc.SetMaxGroundAngle(75f);
        }
        
        public override void AfterTick()
        {
            base.AfterTick();

            if (!Object.HasStateAuthority) return;

            _jumpImpulse = 0;
        }
        
        // FixedUpdateNetwork - not called on proxy clients
        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            kcc.SetGravity(kcc.RealVelocity.y >= 0 ? roomData.settings.upGravity : roomData.settings.downGravity);
            
            if (ProjectSettings.IsGamePaused)
            {
                MoveVelocity = _desiredMoveDirection = Vector3.zero;
                kcc.ResetVelocity();
                kcc.Move(kcc.IsGrounded ? Vector3.zero : new Vector3(0, roomData.settings.downGravity, 0));
                return;
            }

            CalculateDesiredMoveDirection();
            CalculateJumpImpulse();
            CalculateAcceleration();
            CalculateMoveVelocity();

            kcc.Move(MoveVelocity, _jumpImpulse);
        }

        private void CalculateDesiredMoveDirection()
        {
            float currentSpeed = CalculateSpeed();

            Vector3 inputDirection = kcc.TransformRotation * new Vector3(
                input.CurrentInput.MoveDirection.x, 0.0f,
                input.CurrentInput.MoveDirection.y);

            if (inputDirection.sqrMagnitude > 1f)
                inputDirection.Normalize();

            _desiredMoveDirection = roomData.settings.autoRun ? transform.forward : inputDirection;
            _desiredMoveDirection *= currentSpeed;

            if (kcc.ProjectOnGround(_desiredMoveDirection, projectedVector: out Vector3 moveVelocity))
            {
                _desiredMoveDirection =
                    Vector3.ClampMagnitude(moveVelocity.normalized * currentSpeed, currentSpeed);
            }
        }

        private void CalculateJumpImpulse()
        {
            if (!kcc.IsGrounded) return;

            if (input.CurrentInput.Actions.WasPressed(input.PreviousInput.Actions, InputModelData.JUMP_BUTTON))
            {
                _jumpImpulse = roomData.settings.localJumpForce * roomData.settings.jumpFactor;
                JumpAnimation();
            }
        }

        private void CalculateAcceleration()
        {
            if (_desiredMoveDirection == Vector3.zero)
            {
                _acceleration = kcc.IsGrounded
                    ? roomData.settings.groundDeceleration
                    : roomData.settings.airDeceleration;
            }
            else
            {
                _acceleration = kcc.IsGrounded
                    ? roomData.settings.groundAcceleration
                    : roomData.settings.airAcceleration;
            }
        }

        private void CalculateMoveVelocity() => 
            MoveVelocity = Vector3.Lerp(MoveVelocity, _desiredMoveDirection, _acceleration * Runner.DeltaTime);

        private float CalculateSpeed()
        {
            InputModelData curr = input.CurrentInput;
            bool isShiftButtonPressed = curr.Actions.IsSet(InputModelData.SHIFT_BUTTON);
            IsPlayerShifting = roomData.settings.shiftMode && isShiftButtonPressed;
            
            float currentSpeed = IsPlayerShifting ? roomData.settings.runningSpeed : roomData.settings.walkingSpeed;

            return currentSpeed;
        }
    }
}
using Core.Scripts.Game.Player.NetworkInput;
using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.Player.Locomotion
{
    [System.Serializable]
    public struct PlayerTeleportationData
    {
        public Vector3 endPosition;
        public Quaternion endRotation;

        public PlayerTeleportationData(Vector3 endPosition, Quaternion endRotation)
        {
            this.endPosition = endPosition;
            this.endRotation = endRotation;
        }
    }

    public sealed class PlayerBaseMovement : PlayerBaseEffects
    {
        [Networked] private Vector3 MoveVelocity { get; set; }

        private Vector3 _desiredMoveDirection;

        private float _jumpImpulse;
        private float _acceleration;

        private const float BUBBLE_SLOWDOWN_BOOST = 5f;
        private const float ON_GROUND_MIN_THRESHOLD = -20f;

        private bool _wasGroundedLastTick;

        public override void Spawned()
        {
            base.Spawned();

            if (!Object.HasStateAuthority) return;
            kcc.SetMaxGroundAngle(75f);
        }

        public override void BeforeTick()
        {
            _wasGroundedLastTick = kcc.IsGrounded;

            if (!Object.HasStateAuthority) return;
        }

        public override void AfterTick()
        {
            if (kcc.IsGrounded && !_wasGroundedLastTick && kcc.RealVelocity.y < ON_GROUND_MIN_THRESHOLD)
            {
                OnGroundEffect();
            }

            if (!Object.HasStateAuthority) return;

            _jumpImpulse = 0;
        }
        
        // FixedUpdateNetwork - not called on proxy clients
        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            kcc.SetGravity(kcc.RealVelocity.y >= 0 ? photonRoomSettings.upGravity : photonRoomSettings.downGravity);

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

            _desiredMoveDirection = photonRoomSettings.autoRun ? transform.forward : inputDirection;
            _desiredMoveDirection *= currentSpeed;

            if (kcc.ProjectOnGround(_desiredMoveDirection, projectedVector: out Vector3 projectedDesiredMoveVelocity))
            {
                _desiredMoveDirection =
                    Vector3.ClampMagnitude(projectedDesiredMoveVelocity.normalized * currentSpeed, currentSpeed);
            }
        }

        private void CalculateJumpImpulse()
        {
            if (!kcc.IsGrounded) return;

            if (photonRoomSettings.autoBunnyHop)
            {
                _jumpImpulse = photonRoomSettings.localJumpForce * photonRoomSettings.jumpFactor;
                return;
            }

            if (input.CurrentInput.Actions.WasPressed(input.PreviousInput.Actions, InputModelData.JUMP_BUTTON))
            {
                _jumpImpulse = photonRoomSettings.localJumpForce * photonRoomSettings.jumpFactor;
                JumpAnimation();
            }
        }

        private void CalculateAcceleration()
        {
            if (_desiredMoveDirection == Vector3.zero)
            {
                _acceleration = kcc.IsGrounded
                    ? photonRoomSettings.groundDeceleration
                    : photonRoomSettings.airDeceleration;
            }
            else
            {
                _acceleration = kcc.IsGrounded
                    ? photonRoomSettings.groundAcceleration
                    : photonRoomSettings.airAcceleration;
            }
        }

        private void CalculateMoveVelocity() => 
            MoveVelocity = Vector3.Lerp(MoveVelocity, _desiredMoveDirection, _acceleration * Runner.DeltaTime);

        private float CalculateSpeed()
        {
            bool isRunning = photonRoomSettings.shiftMode && input.KeyHandler.IsShifting;
            float currentSpeed = isRunning ? photonRoomSettings.runningSpeed : photonRoomSettings.walkingSpeed;

            return currentSpeed;
        }
    }
}
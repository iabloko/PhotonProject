using Core.Scripts.Game.Player.NetworkInput;
using Fusion;
using Sandbox.Project.Scripts.Infrastructure.ModelData.MovementEffects;
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

        public void PlayerRestart()
        {
            kcc.ResetVelocity();
        }

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

            // _jumpImpulse = JumpPadImpulse > 0 ? JumpPadImpulse : _jumpImpulse;

            StartTeleportation();
        }

        public override void AfterTick()
        {
            if (kcc.IsGrounded && !_wasGroundedLastTick && kcc.RealVelocity.y < ON_GROUND_MIN_THRESHOLD)
            {
                OnGroundEffect();
            }

            if (!Object.HasStateAuthority) return;

            _jumpImpulse = 0;
            // JumpPadImpulse = 0;

            // if (kcc.IsGrounded)
            // {
            //     IsBounced = false;
            //     JumpPadForwardDirection = Vector3.zero;
            // }

            // TryToRecordMovementEffect();
            TryToCompleteTeleportation();
        }

        public override void SaveDataForKccTeleportation(PlayerTeleportationData data)
        {
            // PlayerInfo.PlayerInStatus.ChangePlayerTeleportationStatus(true);
            base.SaveDataForKccTeleportation(data);
        }

        // FixedUpdateNetwork - not called on proxy clients
        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            kcc.SetGravity(kcc.RealVelocity.y >= 0 ? photonRoomSettings.upGravity : photonRoomSettings.downGravity);

            // if (IsPlayerBusy || ProjectSettings.IsGamePaused)
            // {
            //     MoveVelocity = _desiredMoveDirection = Vector3.zero;
            //     kcc.ResetVelocity();
            //     kcc.Move(kcc.IsGrounded ? Vector3.zero : new Vector3(0, photonRoomSettings.downGravity, 0));
            //     return;
            // }

            CalculateDesiredMoveDirection();
            CalculateJumpImpulse();
            CalculateAcceleration();
            CalculateMoveVelocity();

            kcc.Move(MoveVelocity, _jumpImpulse);
        }

        public override void StartMovementEvent(MovementEffectData movementData)
        {
            // CachedInteractionObjectType = movementData.Type;
            base.StartMovementEvent(movementData);

            // if (movementData.Type.Equals(InteractionObjectType.JUMP_PAD))
            //     StartJumPad(movementData);
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
            // if (!kcc.IsGrounded || IsBananaEffect || IsBubbleGumEffect)
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

        private void CalculateMoveVelocity()
        {
            MoveVelocity = Vector3.Lerp(MoveVelocity, _desiredMoveDirection, _acceleration * Runner.DeltaTime);

            // if (IsBounced)
            // {
            //     CalculateWithJumPadForce();
            // }
            // else
            // {
            //     MoveVelocity = Vector3.Lerp(MoveVelocity, _desiredMoveDirection, _acceleration * Runner.DeltaTime);
            // }
        }

        // private void CalculateWithJumPadForce()
        // {
        //     if (JumpPadVelocity.magnitude < 0.1f)
        //     {
        //         IsBounced = false;
        //         JumpPadForwardDirection = Vector3.zero;
        //
        //         MoveVelocity = Vector3.Lerp(MoveVelocity, _desiredMoveDirection, _acceleration * Runner.DeltaTime);
        //     }
        //
        //     float dragFactor;
        //
        //     if (_desiredMoveDirection == Vector3.zero)
        //     {
        //         dragFactor = 1f - photonRoomSettings.jumpPadAirDeceleration * Runner.DeltaTime;
        //         JumpPadVelocity *= dragFactor;
        //     }
        //     else
        //     {
        //         dragFactor = 1f - photonRoomSettings.jumpPadAirDecelerationMovement * Runner.DeltaTime;
        //         JumpPadVelocity = (JumpPadVelocity * dragFactor) + _desiredMoveDirection.normalized;
        //     }
        //
        //     MoveVelocity = JumpPadVelocity;
        // }

        // private void TryToRecordMovementEffect()
        // {
        //     if (CachedInteractionObjectType.Equals(InteractionObjectType.NONE)) return;
        //     CachedInteractionObjectType = InteractionObjectType.NONE;
        // }

        private float CalculateSpeed()
        {
            bool isRunning = photonRoomSettings.shiftMode && input.KeyHandler.IsShifting;
            float currentSpeed = isRunning ? photonRoomSettings.runningSpeed : photonRoomSettings.walkingSpeed;

            // if (IsBananaEffect)
            // {
            //     currentSpeed = 0f;
            // }
            // else
            // {
            //     bool isRunning = photonRoomSettings.shiftMode && input.KeyHandler.IsShifting;
            //
            //     currentSpeed = isRunning ? photonRoomSettings.runningSpeed : photonRoomSettings.walkingSpeed;
            //     currentSpeed *= 1 / (IsBubbleGumEffect ? BUBBLE_SLOWDOWN_BOOST : 1);
            //     currentSpeed *= IsSpeedBoostEffect ? photonRoomSettings.speedUpBoost : 1;
            // }
            return currentSpeed;
        }

        // private void StartJumPad(MovementEffectData data)
        // {
        //     if (photonRoomSettings.jumpFactor.Equals(0) && photonRoomSettings.jumpInertia.Equals(0)) return;
        //
        //     if (data is JumpPadMovementData jumpPadData)
        //     {
        //         float jumpPadStrength = jumpPadData.NetJumpStrength;
        //
        //         JumpPadImpulse = jumpPadStrength * (1.36f - 0.004f * jumpPadStrength);
        //         JumpPadImpulse = Mathf.Max(0f, JumpPadImpulse);
        //
        //         JumpPadForwardDirection = jumpPadData.Child.forward;
        //         JumpPadForwardDirection.y = 0;
        //
        //         JumpPadVelocity = JumpPadForwardDirection * JumpPadImpulse;
        //
        //         IsBounced = true;
        //
        //         MoveVelocity = _desiredMoveDirection = Vector3.zero;
        //         kcc.ResetVelocity();
        //     }
        //     else
        //     {
        //         Debug.LogError($"StartJumPad => {data.GetType().Name}");
        //     }
        // }
    }
}
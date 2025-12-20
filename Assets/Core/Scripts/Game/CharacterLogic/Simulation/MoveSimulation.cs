using Core.Scripts.Game.Infrastructure.ModelData;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using UnityEngine;

namespace Core.Scripts.Game.CharacterLogic.Simulation
{
    public sealed class MoveSimulation
    {
        private readonly ICharacterMotor _motor;
        private readonly ICharacterInput _input;
        private readonly ITimeSource _time;
        private readonly GameplaySettings _gameplay;
        private readonly IProjectSettings _projectSettings;
        private readonly System.Action _onJumpAnim;

        private Vector3 _moveVelocity;
        private Vector3 _desired;
        private float _jumpImpulse;
        private float _accel;

        public MoveSimulation(ICharacterMotor motor, ICharacterInput input, ITimeSource time,
            GameplaySettings gameplay, IProjectSettings projectSettings, System.Action onJumpAnim)
        {
            _motor = motor;
            _input = input;
            _time = time;
            _gameplay = gameplay;
            _projectSettings = projectSettings;
            _onJumpAnim = onJumpAnim;

            _motor.SetMaxGroundAngle(75f);
        }

        public void AfterTick() => _jumpImpulse = 0f;

        public void FixedTick()
        {
            _motor.SetGravity(
                _motor.RealVelocity.y >= 0 ? _gameplay.settings.upGravity : _gameplay.settings.downGravity);

            if (_projectSettings.IsGamePaused)
            {
                _moveVelocity = _desired = Vector3.zero;
                _motor.ResetVelocity();
                _motor.Move(_motor.IsGrounded ? Vector3.zero : new Vector3(0, _gameplay.settings.downGravity, 0));
                return;
            }

            CalcDesired();
            CalcJump();
            CalcAccel();

            _moveVelocity = Vector3.Lerp(_moveVelocity, _desired, _accel * _time.DeltaTime);
            _motor.Move(_moveVelocity, _jumpImpulse);
        }

        private void CalcDesired()
        {
            float speed = _gameplay.settings.autoRun && _input.SprintHeld
                ? _gameplay.settings.runningSpeed
                : _gameplay.settings.walkingSpeed;

            Vector3 inputDir = _motor.TransformRotation * new Vector3(_input.Move.x, 0f, _input.Move.y);
            if (inputDir.sqrMagnitude > 1f) inputDir.Normalize();

            _desired = _gameplay.settings.autoRun ? _motor.TransformDirection : inputDir;
            _desired *= speed;

            if (_motor.ProjectOnGround(_desired, out Vector3 projected))
                _desired = Vector3.ClampMagnitude(projected.normalized * speed, speed);
        }

        private void CalcJump()
        {
            if (!_motor.IsGrounded) return;
            if (!_input.JumpPressed) return;

            _jumpImpulse = _gameplay.settings.localJumpForce * _gameplay.settings.jumpFactor;
            _onJumpAnim?.Invoke();
        }

        private void CalcAccel()
        {
            if (_desired == Vector3.zero)
                _accel = _motor.IsGrounded ? _gameplay.settings.groundDeceleration : _gameplay.settings.airDeceleration;
            else
                _accel = _motor.IsGrounded ? _gameplay.settings.groundAcceleration : _gameplay.settings.airAcceleration;
        }
    }
}
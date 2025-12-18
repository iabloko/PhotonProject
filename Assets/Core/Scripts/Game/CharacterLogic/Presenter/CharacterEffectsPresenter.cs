using UnityEngine;

namespace Core.Scripts.Game.CharacterLogic.Presenter
{
    public sealed class FootPrint
    {
        private readonly ParticleSystem _footprintParticles;

        public FootPrint(ParticleSystem footprintParticles) => _footprintParticles = footprintParticles;

        public void OnPlayerMovement()
        {
            if (_footprintParticles == null) return;
            if (!_footprintParticles.isPlaying) _footprintParticles.Play();
        }

        public void OnUpdateCall()
        {
            if (_footprintParticles == null) return;
            if (_footprintParticles.isPlaying) _footprintParticles.Stop();
        }
    }

    public sealed class MovementEffects
    {
        private FootPrint _footPrintEffect;

        public void CreateMovementEffects(ParticleSystem footprintParticles)
        {
            _footPrintEffect = new FootPrint(footprintParticles);
        }

        public void UpdatePlayerEffects(bool isMoving)
        {
            if (isMoving) _footPrintEffect.OnPlayerMovement();
            else _footPrintEffect.OnUpdateCall();
        }
    }

    public sealed class CharacterEffectsPresenter
    {
        private bool _wasGroundedLastTick;
        private float _lastVerticalVelocity;

        private readonly ICharacterMotor _motor;
        private readonly MovementEffects _movementEffects;
        private readonly ParticleSystem _onGroundParticles;

        private const float ON_GROUND_MIN_THRESHOLD = -25f;

        public CharacterEffectsPresenter(ICharacterMotor motor, ParticleSystem footprintParticles, ParticleSystem onGroundParticles)
        {
            _motor = motor;
            _onGroundParticles = onGroundParticles;

            _movementEffects = new MovementEffects();
            _movementEffects.CreateMovementEffects(footprintParticles);
        }

        public void BeforeTick()
        {
            _wasGroundedLastTick = _motor.IsGrounded;
            _lastVerticalVelocity = _motor.RealVelocity.y;
        }

        public void OnGroundEffect()
        {
            if (!_wasGroundedLastTick && _motor.IsGrounded && _lastVerticalVelocity < ON_GROUND_MIN_THRESHOLD)
            {
                _onGroundParticles.transform.position = _motor.Position;
                _onGroundParticles.Play();
            }
        }

        public void LateUpdate()
        {
            bool moving = _motor.IsGrounded && _motor.RealVelocity.sqrMagnitude > 0.02f;
            _movementEffects.UpdatePlayerEffects(moving);
        }
    }
}

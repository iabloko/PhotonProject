using UnityEngine;

namespace Core.Scripts.Game.CharacterLogic.Presenter
{
    public sealed class MovementEffects
    {
        private readonly ParticleSystem _footprintParticles;

        public MovementEffects(ParticleSystem footprintParticles)
        {
            _footprintParticles = footprintParticles;
        }

        public void PlayFootprintParticles()
        {
            if (!_footprintParticles) return;
            if (!_footprintParticles.isPlaying)
                _footprintParticles.Play();
        }

        public void StopFootprintParticles()
        {
            if (!_footprintParticles) return;
            if (_footprintParticles.isPlaying)
                _footprintParticles.Stop();
        }
    }
    
    public sealed class CharacterEffectsPresenter
    {
        private const float ON_GROUND_MIN_THRESHOLD = -0.5f;
        private const float MOVING_MIN_SQR_SPEED = 0.02f;

        private readonly ICharacterMotor _motor;
        private readonly MovementEffects _movementEffects;
        private readonly ParticleSystem _onGroundParticles;

        private bool _wasGroundedLastTick;
        private float _lastVerticalVelocity;

        public CharacterEffectsPresenter(
            ICharacterMotor motor,
            ParticleSystem footprintParticles,
            ParticleSystem onGroundParticles)
        {
            _motor = motor;
            _movementEffects = new MovementEffects(footprintParticles);
            _onGroundParticles = onGroundParticles;
        }

        public void BeforeTick()
        {
            _wasGroundedLastTick = _motor.IsGrounded;
            _lastVerticalVelocity = _motor.RealVelocity.y;
        }

        public void OnGroundEffect()
        {
            if (!_onGroundParticles) return;

            if (!_wasGroundedLastTick && _motor.IsGrounded && _lastVerticalVelocity < ON_GROUND_MIN_THRESHOLD)
            {
                _onGroundParticles.transform.position = _motor.Position;
                _onGroundParticles.Play();
            }
        }

        public void LateUpdate()
        {
            bool moving = _motor.IsGrounded && _motor.RealVelocity.sqrMagnitude > MOVING_MIN_SQR_SPEED;
            if (moving) _movementEffects.PlayFootprintParticles();
            else _movementEffects.StopFootprintParticles();
        }
    }
}

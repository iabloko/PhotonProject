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
            if (!_footprintParticles.isPlaying)
                _footprintParticles.Play();
        }

        public void StopFootprintParticles()
        {
            if (_footprintParticles.isPlaying)
                _footprintParticles.Stop();
        }
    }

    public sealed class CharacterEffectsPresenter
    {
        private bool _wasGroundedLastTick;
        private float _lastVerticalVelocity;

        private readonly ICharacterMotor _motor;
        private readonly ICharacterInput _input;
        private readonly MovementEffects _movementEffects;
        private readonly ParticleSystem _onGroundParticles;

        private const float ON_GROUND_MIN_THRESHOLD = -25f;

        public CharacterEffectsPresenter(
            ICharacterMotor motor, ICharacterInput input,
            ParticleSystem footprintParticles,
            ParticleSystem onGroundParticles)
        {
            _motor = motor;
            _input = input;
            _onGroundParticles = onGroundParticles;
            _movementEffects = new MovementEffects(footprintParticles);
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
            bool moving = _motor.IsGrounded && _input.SprintHeld && _motor.RealVelocity.sqrMagnitude > 0.02f;
            if (moving) _movementEffects.PlayFootprintParticles();
            else _movementEffects.StopFootprintParticles();
        }
    }
}
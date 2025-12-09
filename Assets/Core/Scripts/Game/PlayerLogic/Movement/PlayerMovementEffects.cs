using UnityEngine;

namespace Core.Scripts.Game.PlayerLogic.Movement
{
    public sealed class FootPrint
    {
        private readonly ParticleSystem _footprintParticles;
        
        public FootPrint(ParticleSystem footprintParticles)
        {
            _footprintParticles = footprintParticles;
        }

        public void OnPlayerMovement()
        {
            if (_footprintParticles.isPlaying) return;
            _footprintParticles.Play();
        }

        public void OnPlayerStop()
        {
            if (_footprintParticles.isStopped) return;
            _footprintParticles.Stop();
        }

        public void OnUpdateCall()
        {
        }
    }
    
    public sealed class MovementEffects
    {
        private FootPrint _footPrintEffect;
        
        public void CreateMovementEffects(ParticleSystem footprintParticles)
        {
            _footPrintEffect = new FootPrint(footprintParticles);
        }
        
        public void UpdatePlayerEffects(bool isPlayerMoving)
        {
            if (isPlayerMoving)
            {
                EffectsOnMovementLogic();
            }
            else
            {
                EffectsOnStopMovementLogic();
            }

            EffectsOnUpdateLogic();
        }

        private void EffectsOnStopMovementLogic() => _footPrintEffect.OnPlayerStop();
        private void EffectsOnMovementLogic() => _footPrintEffect.OnPlayerMovement();
        private void EffectsOnUpdateLogic() => _footPrintEffect.OnUpdateCall();
    }
    
    public sealed class PlayerMovementEffects
    {
        private PlayerTeleportationData _playerTeleportationData;

        private bool _wasGroundedLastTick;
        private float _lastVerticalVelocity;

        private readonly PlayerContext _context;
        private readonly MovementEffects _movementEffects;
        private readonly ParticleSystem _onGroundParticles;
        private const float ON_GROUND_MIN_THRESHOLD = -25f;
        
        public PlayerMovementEffects(
            ParticleSystem footprintParticles, ParticleSystem onGroundParticles, PlayerContext context)
        {
            _context = context;
            _onGroundParticles = onGroundParticles;
            
            _movementEffects = new MovementEffects();
            _movementEffects.CreateMovementEffects(footprintParticles);
        }

        public void OnGroundEffect()
        {
            if (!_wasGroundedLastTick && _context.Kcc.IsGrounded && _lastVerticalVelocity < ON_GROUND_MIN_THRESHOLD)
            {
                Vector3 onGroundPosition = _context.Kcc.Position;
                _onGroundParticles.transform.position = onGroundPosition;
                _onGroundParticles.Play();
            }
        }

        public void BeforeTick()
        {
            _wasGroundedLastTick = _context.Kcc.IsGrounded;
            _lastVerticalVelocity = _context.Kcc.RealVelocity.y;
        }

        public void LateUpdate()
        {
            bool showPlayerEffect = _context.IsPlayerShifting && _context.Kcc.IsGrounded;
            _movementEffects.UpdatePlayerEffects(showPlayerEffect);
        }
    }
}
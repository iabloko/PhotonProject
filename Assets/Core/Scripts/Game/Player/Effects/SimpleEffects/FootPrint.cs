using UnityEngine;

namespace Core.Scripts.Game.Player.Effects.SimpleEffects
{
    public sealed class FootPrint : IPlayerEffect
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
}
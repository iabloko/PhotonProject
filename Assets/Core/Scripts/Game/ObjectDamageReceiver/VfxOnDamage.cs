using UnityEngine;

namespace Core.Scripts.Game.ObjectDamageReceiver
{
    public sealed class VfxOnDamage
    {
        private readonly ParticleSystem _hitParticles;

        public VfxOnDamage(ParticleSystem hitParticles)
        {
            _hitParticles = hitParticles;
        }

        public void OnDamage(in DamageInfo info)
        {
            if (_hitParticles == null) return;

            // _hitParticles.transform.position = info.HitPoint;
            _hitParticles.Play();
        }
    }
}
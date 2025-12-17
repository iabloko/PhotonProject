using Core.Scripts.Game.PlayerLogic;
using Core.Scripts.Game.ScriptableObjects.Sound;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.ObjectDamageReceiver
{
    public interface IDamageable
    {
        public bool IsAlive { get; }
        public void TakeDamage(in DamageInfo info);
    }

    public sealed class DamageReceiver : MonoBehaviour, IDamageable
    {
        public bool IsAlive { get; } = true;

        [SerializeField] private Animator animator;
        // [SerializeField] private PlayerController player;
        [SerializeField] private ParticleSystem hitParticles;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private SoundSettings damageSoundSettings;

        private HealthOnDamage _health;
        private SfxOnDamage _sfx;
        private VfxOnDamage _vfx;

        private void Awake()
        {
            // _health = new HealthOnDamage(
            //     getHealth: () => player.CurrentHealth,
            //     setHealth: h => player.ChangeHealth(h),
            //     maxHealth: 100,
            //     onDeath: OnDeath);

            _sfx = new SfxOnDamage(audioSource, damageSoundSettings);
            _vfx = new VfxOnDamage(hitParticles);
        }

        // private void OnDeath(DamageInfo info) => player.RPC_PlayerDeathLogic();

        void IDamageable.TakeDamage(in DamageInfo info)
        {
            // if (!player.HasStateAuthority) return;
            //
            // _health.OnDamage(info);
            // _sfx.OnDamage(info);
            // _vfx.OnDamage(info);
        }

        [Button(ButtonSizes.Gigantic)]
        private void ManualTakeDamage()
        {
            DamageInfo info = new();
            ((IDamageable)this).TakeDamage(info);
        }
    }
}
using System;
using Core.Scripts.Game.CharacterLogic;
using Core.Scripts.Game.Combat.DamageCalculator;
using Core.Scripts.Game.Combat.Data;
using Core.Scripts.Game.Combat.Events;
using Core.Scripts.Game.Combat.PercentageArmorCalculator;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.Game.Combat.Presenters
{
    /// <summary>
    /// Конфигурация для создания комбат-систем персонажа.
    /// </summary>
    public sealed class CombatConfig
    {
        public ICharacterMotor Motor { get; set; }
        public ITimeSource Time { get; set; }
        public IWeaponRegistry WeaponRegistry { get; set; }

        public Func<int> GetWeaponId { get; set; }
        public Func<NetworkId> GetNetworkId { get; set; }
        public Func<int> GetOwnLayer { get; set; }

        public Func<HealthNetwork> GetHealth { get; set; }
        public Action<HealthNetwork> SetHealth { get; set; }

        public int InitialMaxHealth { get; set; } = 100;
        public int InitialArmor { get; set; } = 0;

        public bool HasStateAuthority { get; set; }

        // Презентеры (опционально)
        public Animator Animator { get; set; }
        public ParticleSystem HitParticles { get; set; }
        // public Image HealthBar { get; set; }
        // public CanvasGroup DamageFlash { get; set; }
        public GameObject RagdollRoot { get; set; }

        // Callbacks
        public Action<DamageEvent> OnDamageDealt { get; set; }
        public Action<DamageEvent> OnDamageReceived { get; set; }
        public Action OnDeath { get; set; }
    }

    /// <summary>
    /// Результат создания комбат-систем.
    /// </summary>
    public sealed class CombatRuntime : IDisposable
    {
        public HealthSimulation HealthSim { get; }
        public AttackSimulation AttackSim { get; }

        // public HealthPresenter HealthPresenter { get; }
        public DamageEffectsPresenter DamageEffectsPresenter { get; }
        public DeathPresenter DeathPresenter { get; }

        public CombatRuntime(
            HealthSimulation healthSim,
            AttackSimulation attackSim,
            // HealthPresenter healthPresenter,
            DamageEffectsPresenter damageEffectsPresenter,
            DeathPresenter deathPresenter)
        {
            HealthSim = healthSim;
            AttackSim = attackSim;
            // HealthPresenter = healthPresenter;
            DamageEffectsPresenter = damageEffectsPresenter;
            DeathPresenter = deathPresenter;
        }

        public void LateUpdate(float deltaTime, float normalizedHealth)
        {
            // HealthPresenter.Update(deltaTime);
            // HealthPresenter.SetHealth(normalizedHealth);
        }

        public void Dispose()
        {
        }

        public void PlayDeath() => DeathPresenter.PlayDeath();

        public void TryAttack()
        {
           bool result = AttackSim.TryAttack();
           Debug.Log($"[Player] Try Attack Result - {result}");
        }

        public void SetHealth(float healthNormalizedHealth)
        {
            // HealthPresenter.SetHealth(healthNormalizedHealth);
        }
    }
    
    public sealed class CombatFactory
    {
        private readonly int _damageableLayer;

        public CombatFactory(int damageableLayer) => _damageableLayer = damageableLayer;

        public CombatRuntime Create(CombatConfig config)
        {
            HealthSimulation healthSim = null;
            AttackSimulation attackSim = null;

            // HealthPresenter healthPresenter = CreateHealthPresenter(config);
            DamageEffectsPresenter damageEffectsPresenter = CreateDamageEffectsPresenter(config);
            DeathPresenter deathPresenter = CreateDeathPresenter(config);

            if (config.HasStateAuthority)
            {
                healthSim = CreateHealthSimulation(config, damageEffectsPresenter, deathPresenter);
                attackSim = CreateAttackSimulation(config);
            }

            return new CombatRuntime(
                healthSim,
                attackSim,
                // healthPresenter,
                damageEffectsPresenter,
                deathPresenter);
        }

        private HealthSimulation CreateHealthSimulation(
            CombatConfig config,
            DamageEffectsPresenter effectsPresenter,
            DeathPresenter deathPresenter)
        {
            return new HealthSimulation(
                config.GetHealth,
                config.SetHealth,
                onDamageReceived: damageEvent =>
                {
                    effectsPresenter?.PlayHitEffect(damageEvent);
                    config.OnDamageReceived?.Invoke(damageEvent);
                },
                onDeath: () =>
                {
                    deathPresenter?.PlayDeath();
                    config.OnDeath?.Invoke();
                });
        }

        private AttackSimulation CreateAttackSimulation(CombatConfig config)
        {
            IHitDetector hitDetector = new OverlapHitDetector(
                bufferSize: 16, damageableLayer: _damageableLayer);

            IDamageCalculator damageCalculator = new StandardDamageCalculator(config.WeaponRegistry);

            return new AttackSimulation(
                hitDetector,
                damageCalculator,
                config.WeaponRegistry,
                config.Motor,
                config.Time,
                config.GetWeaponId,
                config.GetNetworkId,
                config.GetOwnLayer,
                config.OnDamageDealt);
        }

        // private HealthPresenter CreateHealthPresenter(CombatConfig config)
        // {
        //     return new HealthPresenter(config.HealthBar, config.DamageFlash);
        // }

        private DamageEffectsPresenter CreateDamageEffectsPresenter(CombatConfig config)
        {
            return new DamageEffectsPresenter(config.HitParticles, null);
        }

        private DeathPresenter CreateDeathPresenter(CombatConfig config)
        {
            return new DeathPresenter(config.Animator, config.RagdollRoot);
        }
    }
}

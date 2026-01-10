using System;
using Core.Scripts.Game.Combat.Data;

namespace Core.Scripts.Game.Combat.PercentageArmorCalculator
{
    /// <summary>
    /// Симуляция здоровья персонажа.
    /// Управляет получением урона, смертью и восстановлением.
    /// Работает только на State Authority.
    /// </summary>
    public sealed class HealthSimulation
    {
        private readonly Func<HealthNetwork> _getHealth;
        private readonly Action<HealthNetwork> _setHealth;
        private readonly Action<Data.DamageEvent> _onDamageReceived;
        private readonly Action _onDeath;

        public HealthSimulation(
            Func<HealthNetwork> getHealth,
            Action<HealthNetwork> setHealth,
            Action<Data.DamageEvent> onDamageReceived = null,
            Action onDeath = null)
        {
            _getHealth = getHealth;
            _setHealth = setHealth;
            _onDamageReceived = onDamageReceived;
            _onDeath = onDeath;
        }

        public bool IsDead => _getHealth().IsDead;
        public int CurrentHealth => _getHealth().current;
        public int MaxHealth => _getHealth().max;
        public int Armor => _getHealth().armor;

        public void ApplyDamage(Data.DamageEvent damageEvent)
        {
            HealthNetwork health = _getHealth();

            if (health.IsDead)
                return;

            health.current = Math.Max(0, health.current - damageEvent.Result.FinalDamage);
            _setHealth(health);

            _onDamageReceived?.Invoke(damageEvent);

            if (health.IsDead)
            {
                _onDeath?.Invoke();
            }
        }

        public void Heal(int amount)
        {
            if (amount <= 0) return;

            HealthNetwork health = _getHealth();

            if (health.IsDead)
                return;

            health.current = Math.Min(health.max, health.current + amount);
            _setHealth(health);
        }

        public void SetArmor(int armor)
        {
            HealthNetwork health = _getHealth();
            health.armor = Math.Max(0, armor);
            _setHealth(health);
        }

        public void Reset()
        {
            HealthNetwork health = _getHealth();
            health.current = health.max;
            _setHealth(health);
        }

        public void Reset(int maxHealth, int armor)
        {
            HealthNetwork health = new HealthNetwork(maxHealth, armor);
            _setHealth(health);
        }
    }
}

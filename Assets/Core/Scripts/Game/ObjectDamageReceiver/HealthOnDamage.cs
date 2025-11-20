using System;
using UnityEngine;

namespace Core.Scripts.Game.ObjectDamageReceiver
{
    public sealed class HealthOnDamage
    {
        private readonly Func<int> _getHealth;
        private readonly Action<int> _setHealth;
        private readonly int _maxHealth;
        private readonly Action<DamageInfo> _onDeath;

        public bool IsDead => _getHealth() <= 0;

        public HealthOnDamage(Func<int> getHealth, Action<int> setHealth, int maxHealth, Action<DamageInfo> onDeath)
        {
            _getHealth = getHealth;
            _setHealth = setHealth;
            _maxHealth = maxHealth;
            _onDeath = onDeath;
        }

        public void OnDamage(in DamageInfo info)
        {
            int current = _getHealth();
            if (current <= 0) return;

            int next = Mathf.Clamp(current - info.Amount, 0, _maxHealth);
            _setHealth(next);

            if (next == 0) _onDeath?.Invoke(info);
        }
    }
}
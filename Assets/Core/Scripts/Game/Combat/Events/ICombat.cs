using Core.Scripts.Game.Combat.Data;
using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.Combat.Events
{
    /// <summary>
    /// Интерфейс для объектов, способных получать урон.
    /// Реализуется Player, NPC и любыми разрушаемыми объектами.
    /// </summary>
    public interface IDamageable
    {
        NetworkId NetworkId { get; }
        HealthNetwork Health { get; }
        Transform Transform { get; }
        bool IsDead { get; }

        /// <summary>
        /// Применяет урон к объекту. Вызывается только на State Authority.
        /// </summary>
        void ApplyDamage(Data.DamageEvent damageEvent);

        /// <summary>
        /// Возвращает текущее значение брони для расчёта урона.
        /// </summary>
        int GetArmor();
    }

    /// <summary>
    /// Интерфейс для объектов, способных наносить урон.
    /// Реализуется Player, NPC и источниками урона окружения.
    /// </summary>
    public interface IDamageDealer
    {
        NetworkId NetworkId { get; }
        Transform Transform { get; }

        /// <summary>
        /// Возвращает текущий контекст атаки на основе оружия и состояния.
        /// </summary>
        AttackContext GetAttackContext();
    }

    /// <summary>
    /// Интерфейс для получения конфигов оружия по ID.
    /// </summary>
    public interface IWeaponRegistry
    {
        bool TryGetConfig(int weaponId, out WeaponCombatConfig config);
    }

    /// <summary>
    /// Интерфейс детектора попаданий.
    /// Позволяет подменять реализацию (overlap, raycast, etc.)
    /// </summary>
    public interface IHitDetector
    {
        /// <summary>
        /// Находит все IDamageable в зоне атаки.
        /// </summary>
        int DetectHits(AttackContext context, IDamageable[] buffer, int excludeLayer = 0);
    }

    /// <summary>
    /// Интерфейс калькулятора урона.
    /// Позволяет расширять логику расчёта (баффы, дебаффы, etc.)
    /// </summary>
    public interface IDamageCalculator
    {
        DamageResult Calculate(AttackContext context, IDamageable target);
    }
}

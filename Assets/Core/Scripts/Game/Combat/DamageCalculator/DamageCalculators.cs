using Core.Scripts.Game.Combat.Data;
using Core.Scripts.Game.Combat.Events;
using UnityEngine;

namespace Core.Scripts.Game.Combat.DamageCalculator
{
    /// <summary>
    /// Стандартный калькулятор урона.
    /// Формула: FinalDamage = Max(0, BaseDamage - Max(0, Armor - ArmorPenetration))
    /// </summary>
    public sealed class StandardDamageCalculator : IDamageCalculator
    {
        private readonly IWeaponRegistry _weaponRegistry;

        public StandardDamageCalculator(IWeaponRegistry weaponRegistry)
        {
            _weaponRegistry = weaponRegistry;
        }

        public DamageResult Calculate(AttackContext context, IDamageable target)
        {
            int baseDamage = context.BaseDamage;
            int armorPenetration = 0;

            if (_weaponRegistry.TryGetConfig(context.WeaponId, out WeaponCombatConfig config))
            {
                armorPenetration = config.ArmorPenetration;
            }

            int targetArmor = target.GetArmor();
            int effectiveArmor = Mathf.Max(0, targetArmor - armorPenetration);
            int armorReduction = Mathf.Min(effectiveArmor, baseDamage);

            return DamageResult.Create(baseDamage, armorReduction, target.Health.current);
        }
    }

    /// <summary>
    /// Калькулятор с процентной редукцией от брони.
    /// Формула: FinalDamage = BaseDamage * (1 - ArmorReductionPercent)
    /// ArmorReductionPercent = Armor / (Armor + ArmorConstant)
    /// </summary>
    public sealed class PercentageArmorCalculator : IDamageCalculator
    {
        private readonly IWeaponRegistry _weaponRegistry;
        private readonly float _armorConstant;

        public PercentageArmorCalculator(IWeaponRegistry weaponRegistry, float armorConstant = 100f)
        {
            _weaponRegistry = weaponRegistry;
            _armorConstant = armorConstant;
        }

        public DamageResult Calculate(AttackContext context, IDamageable target)
        {
            int baseDamage = context.BaseDamage;
            int armorPenetration = 0;

            if (_weaponRegistry.TryGetConfig(context.WeaponId, out WeaponCombatConfig config))
            {
                armorPenetration = config.ArmorPenetration;
            }

            int targetArmor = target.GetArmor();
            int effectiveArmor = Mathf.Max(0, targetArmor - armorPenetration);

            float reductionPercent = effectiveArmor / (effectiveArmor + _armorConstant);
            int armorReduction = Mathf.RoundToInt(baseDamage * reductionPercent);

            return DamageResult.Create(baseDamage, armorReduction, target.Health.current);
        }
    }
}

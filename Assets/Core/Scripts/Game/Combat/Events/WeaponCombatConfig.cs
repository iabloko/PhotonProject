using Core.Scripts.Game.Combat.Data;
using Core.Scripts.Game.GamePlay.UsableItems;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.Combat.Events
{
    [CreateAssetMenu(fileName = "WeaponCombatConfig", menuName = "Game/Combat/Weapon Config")]
    public sealed class WeaponCombatConfig : ScriptableObject
    {
        [Title("Weapon", "Base Data"), SerializeField, InlineEditor]
        private Weapon _weapon;

        [Title("Attack Type"), SerializeField]
        private AttackType _attackType = AttackType.Melee;

        [Title("Damage"), SerializeField, Min(1)]
        private int _baseDamage = 10;
        [SerializeField, Min(0)] private int _armorPenetration = 0;

        [Title("Melee Settings"), ShowIf(nameof(_attackType), AttackType.Melee)]
        [SerializeField, Min(0.1f)]
        private float _meleeRange = 2f;
        [SerializeField, Min(0.1f)] private float _meleeRadius = 0.5f;
        [SerializeField, Min(0f)] private float _meleeAngle = 90f;

        [Title("Timing"), SerializeField, Min(0.1f)]
        private float _attackCooldown = 0.5f;
        [SerializeField, Min(0f)] private float _damageDelay = 0.1f;

        public int Id => _weapon.id;
        public string DisplayName => _weapon.itemName;
        public AttackType AttackType => _attackType;
        public int BaseDamage => _baseDamage;
        public int ArmorPenetration => _armorPenetration;

        public float MeleeRange => _meleeRange;
        public float MeleeRadius => _meleeRadius;
        public float MeleeAngle => _meleeAngle;

        public float AttackCooldown => _attackCooldown;
        public float DamageDelay => _damageDelay;
        
        public override string ToString() => $"Weapon Combat Config: {DisplayName} | {Id}";
    }
}
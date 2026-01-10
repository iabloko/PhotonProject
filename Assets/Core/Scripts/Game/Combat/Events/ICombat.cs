using Core.Scripts.Game.Combat.Data;
using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.Combat.Events
{
    public interface IDamageable
    {
        public NetworkId NetworkId { get; }
        public HealthNetwork Health { get; }
        public Transform Transform { get; }
        public bool IsDead { get; }
        
        public void ApplyDamage(DamageEvent damageEvent);
        
        public int GetArmor();
    }
    
    public interface IDamageDealer
    {
        public NetworkId NetworkId { get; }
        public Transform Transform { get; }
        
        public AttackContext GetAttackContext();
    }
    
    public interface IWeaponRegistry
    {
        public bool TryGetConfig(int weaponId, out WeaponCombatConfig config);
    }
    
    public interface IHitDetector
    {
        public int DetectHits(AttackContext context, IDamageable[] buffer, int excludeLayer = 0);
    }
    
    public interface IDamageCalculator
    {
        public DamageResult Calculate(AttackContext context, IDamageable target);
    }
}

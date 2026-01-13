using System;
using Core.Scripts.Game.CharacterLogic;
using Core.Scripts.Game.Combat.Data;
using Core.Scripts.Game.Combat.Events;
using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.Combat.PercentageArmorCalculator
{
    public sealed class AttackSimulation
    {
        private readonly IHitDetector _hitDetector;
        private readonly IDamageCalculator _damageCalculator;
        private readonly IWeaponRegistry _weaponRegistry;
        private readonly ICharacterMotor _motor;
        private readonly ITimeSource _time;

        private readonly Func<int> _getWeaponId;
        private readonly Func<NetworkId> _getNetworkId;
        private readonly Func<int> _getOwnLayer;

        private readonly IDamageable[] _hitBuffer;
        private readonly Action<DamageEvent> _onDamageDealt;

        private int _lastAttackTick;

        public AttackSimulation(
            IHitDetector hitDetector,
            IDamageCalculator damageCalculator,
            IWeaponRegistry weaponRegistry,
            ICharacterMotor motor,
            ITimeSource time,
            Func<int> getWeaponId,
            Func<NetworkId> getNetworkId,
            Func<int> getOwnLayer,
            Action<DamageEvent> onDamageDealt = null,
            int hitBufferSize = 16)
        {
            _hitDetector = hitDetector;
            _damageCalculator = damageCalculator;
            _weaponRegistry = weaponRegistry;
            _motor = motor;
            _time = time;
            _getWeaponId = getWeaponId;
            _getNetworkId = getNetworkId;
            _getOwnLayer = getOwnLayer;
            _onDamageDealt = onDamageDealt;
            _hitBuffer = new IDamageable[hitBufferSize];
        }
        
        public bool TryAttack()
        {
            int weaponId = _getWeaponId();
            
            if (!_weaponRegistry.TryGetConfig(weaponId, out WeaponCombatConfig config))
                return false;
            
            if (!CanAttack(config))
                return false;
            
            AttackContext context = CreateAttackContext(config);
            ExecuteAttack(context);
            
            _lastAttackTick = _time.Tick;
            return true;
        }

        private bool CanAttack(WeaponCombatConfig config)
        {
            int ticksSinceLastAttack = _time.Tick - _lastAttackTick;
            float timeSinceLastAttack = ticksSinceLastAttack * _time.DeltaTime;

            return timeSinceLastAttack >= config.AttackCooldown;
        }

        private AttackContext CreateAttackContext(WeaponCombatConfig config)
        {
            Vector3 origin = _motor.Position + Vector3.up * 1.2f;
            Vector3 direction = _motor.TransformDirection;

            return AttackContext.CreateMelee(
                _getNetworkId(),
                config.Id,
                config.BaseDamage,
                origin,
                direction,
                config.MeleeRange,
                config.MeleeRadius,
                _time.Tick);
            
            // if (config.AttackType == AttackType.Melee)
            // {
            //     return AttackContext.CreateMelee(
            //         _getNetworkId(),
            //         config.Id,
            //         config.BaseDamage,
            //         origin,
            //         direction,
            //         config.MeleeRange,
            //         config.MeleeRadius,
            //         _time.Tick);
            // }

            // return AttackContext.CreateRanged(
            //     _getNetworkId(),
            //     config.Id,
            //     config.BaseDamage,
            //     origin,
            //     direction,
            //     config.ProjectileRange,
            //     _time.Tick);
        }

        private void ExecuteAttack(AttackContext context)
        {
            int excludeLayer = 1 << _getOwnLayer();
            int hitCount = _hitDetector.DetectHits(context, _hitBuffer, excludeLayer);
            
            for (int i = 0; i < hitCount; i++)
            {
                IDamageable target = _hitBuffer[i];

                if (target.NetworkId == _getNetworkId()) continue;

                DamageResult result = _damageCalculator.Calculate(context, target);

                Vector3 hitPoint = target.Transform.position;
                Vector3 hitDirection = (hitPoint - context.Origin).normalized;

                DamageEvent damageEvent = new()
                {
                    VictimId = target.NetworkId,
                    AttackerId = context.AttackerId,
                    Result = result,
                    HitPoint = hitPoint,
                    HitDirection = hitDirection,
                    Tick = context.Tick
                };
                
                target.ApplyDamage(damageEvent);
                _onDamageDealt?.Invoke(damageEvent);
            }
        }
    }
}

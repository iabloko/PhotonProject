using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.Combat.Data
{
    /// <summary>
    /// Тип атаки - влияет на способ детекции попаданий
    /// </summary>
    public enum AttackType : byte
    {
        Melee = 0,
        Ranged = 1
    }

    /// <summary>
    /// Результат расчёта урона
    /// </summary>
    public struct DamageResult
    {
        public int RawDamage;
        public int ArmorReduction;
        public int FinalDamage;
        public bool WasLethal;

        public static DamageResult Create(int raw, int armorReduction, int currentHealth)
        {
            int final = Mathf.Max(0, raw - armorReduction);
            return new DamageResult
            {
                RawDamage = raw,
                ArmorReduction = armorReduction,
                FinalDamage = final,
                WasLethal = final >= currentHealth
            };
        }
    }

    /// <summary>
    /// Контекст атаки для передачи между системами
    /// </summary>
    public struct AttackContext
    {
        public NetworkId AttackerId;
        public int WeaponId;
        public int BaseDamage;
        public AttackType AttackType;
        public Vector3 Origin;
        public Vector3 Direction;
        public float Range;
        public float Radius;
        public int Tick;

        public static AttackContext CreateMelee(
            NetworkId attackerId,
            int weaponId,
            int baseDamage,
            Vector3 origin,
            Vector3 direction,
            float range,
            float radius,
            int tick)
        {
            return new AttackContext
            {
                AttackerId = attackerId,
                WeaponId = weaponId,
                BaseDamage = baseDamage,
                AttackType = AttackType.Melee,
                Origin = origin,
                Direction = direction,
                Range = range,
                Radius = radius,
                Tick = tick
            };
        }

        public static AttackContext CreateRanged(
            NetworkId attackerId,
            int weaponId,
            int baseDamage,
            Vector3 origin,
            Vector3 direction,
            float range,
            int tick)
        {
            return new AttackContext
            {
                AttackerId = attackerId,
                WeaponId = weaponId,
                BaseDamage = baseDamage,
                AttackType = AttackType.Ranged,
                Origin = origin,
                Direction = direction,
                Range = range,
                Radius = 0f,
                Tick = tick
            };
        }
    }
    
    public struct DamageEvent
    {
        public NetworkId VictimId;
        public NetworkId AttackerId;
        public DamageResult Result;
        public Vector3 HitPoint;
        public Vector3 HitDirection;
        public int Tick;

        public override string ToString() =>
            $"Damage Event: {VictimId} | {AttackerId} | {HitPoint} | {HitDirection} | {Tick}";
    }

    /// <summary>
    /// Сетевые данные здоровья для синхронизации
    /// </summary>
    [System.Serializable]
    public struct HealthNetwork : INetworkStruct
    {
        public int current;
        public int max;
        public int armor;

        public HealthNetwork(int max, int armor)
        {
            current = max;
            this.max = max;
            this.armor = armor;
        }

        public bool IsDead => current <= 0;
        
        public float NormalizedHealth => max > 0 ? (float)current / max : 0f;
    }
}

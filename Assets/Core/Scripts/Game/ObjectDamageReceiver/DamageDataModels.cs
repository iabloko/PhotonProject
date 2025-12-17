using UnityEngine;

namespace Core.Scripts.Game.ObjectDamageReceiver
{
    public enum DamageType
    {
        Trap,
        Bullet,
        Fall,
    }
    
    public readonly struct DamageInfo
    {
        public readonly int Amount;
        public readonly Vector3 HitPoint;
        public readonly Vector3 HitNormal;
        public readonly DamageType Type;

        public DamageInfo(int amount, Vector3 hitPoint, Vector3 hitNormal, DamageType type)
        {
            Amount = amount;
            HitPoint = hitPoint;
            HitNormal = hitNormal;
            Type = type;
        }
    }
}
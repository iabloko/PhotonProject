using Core.Scripts.Game.Combat.Data;
using Core.Scripts.Game.Combat.Events;
using UnityEngine;

namespace Core.Scripts.Game.Combat.DamageCalculator
{
    public sealed class OverlapHitDetector : IHitDetector
    {
        private readonly Collider[] _colliderBuffer;
        private readonly int _damageableLayer;

        public OverlapHitDetector(int bufferSize = 32, int damageableLayer = 0)
        {
            _colliderBuffer = new Collider[bufferSize];
            _damageableLayer = damageableLayer;
        }

        public int DetectHits(AttackContext context, IDamageable[] buffer, int excludeLayer = 0)
        {
            return context.AttackType == AttackType.Melee
                ? DetectMelee(context, buffer, excludeLayer)
                : DetectRanged(context, buffer, excludeLayer);
        }

        private int DetectMelee(AttackContext context, IDamageable[] buffer, int excludeLayer)
        {
            int layerMask = _damageableLayer != 0 
                ? (1 << _damageableLayer) & ~excludeLayer 
                : ~excludeLayer;
            
            Vector3 castOrigin = context.Origin + context.Direction * (context.Radius * 0.5f);
            
            int count = Physics.OverlapSphereNonAlloc(
                castOrigin,
                context.Range + context.Radius,
                _colliderBuffer,
                layerMask,
                QueryTriggerInteraction.Ignore);
            
            int hitCount = 0;

            for (int i = 0; i < count && hitCount < buffer.Length; i++)
            {
                if (!TryGetDamageable(_colliderBuffer[i], out IDamageable damageable))
                    continue;

                if (damageable.IsDead)
                    continue;

                Vector3 toTarget = damageable.Transform.position - context.Origin;
                float distance = toTarget.magnitude;

                if (distance > context.Range + context.Radius)
                    continue;

                if (distance > 0.01f)
                {
                    float angle = Vector3.Angle(context.Direction, toTarget.normalized);
                    if (angle > 90f)
                        continue;
                }

                buffer[hitCount++] = damageable;
            }

            return hitCount;
        }

        private int DetectRanged(AttackContext context, IDamageable[] buffer, int excludeLayer)
        {
            int layerMask = _damageableLayer != 0 
                ? (1 << _damageableLayer) & ~excludeLayer 
                : ~excludeLayer;
            
            float sphereRadius = Mathf.Max(0.1f, context.Radius);

            RaycastHit[] hits = Physics.SphereCastAll(
                context.Origin,
                sphereRadius,
                context.Direction,
                context.Range,
                layerMask,
                QueryTriggerInteraction.Ignore);

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            int hitCount = 0;

            for (int i = 0; i < hits.Length && hitCount < buffer.Length; i++)
            {
                if (!TryGetDamageable(hits[i].collider, out IDamageable damageable))
                    continue;

                if (damageable.IsDead)
                    continue;

                buffer[hitCount++] = damageable;
            }

            return hitCount;
        }

        private bool TryGetDamageable(Collider collider, out IDamageable damageable)
        {
            Debug.Log($"[OverlapHitDetector]: Try Get Damageable {collider.transform.parent.name}");
            damageable = collider.transform.parent.GetComponentInParent<IDamageable>();
            return damageable != null;
        }
    }
}

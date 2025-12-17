using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.ObjectDamageReceiver
{
    public sealed class DamageDealer : NetworkBehaviour
    {
        public int amount;
        public Vector3 hitPoint;
        public Vector3 hitNormal;
        public DamageType type;

        [SerializeField] private Animator animator;
        private DamageInfo _damageInfo;
        private Transform _otherParent;
        private const string STATE_NAME = "StartGroundTrap";

        public override void Spawned()
        {
            base.Spawned();
            _damageInfo = new DamageInfo(amount, hitPoint, hitNormal, type);
        }

        private void OnTriggerEnter(Collider other)
        {
            _otherParent = other.transform.parent;
            animator.Play(STATE_NAME);
        }

        public void TrapDealDamage()
        {
            if (_otherParent == null) return;
            _otherParent.TryGetComponent(out IDamageable damageable);
            damageable?.TakeDamage(_damageInfo);
        }
    }
}
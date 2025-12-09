using Core.Scripts.Game.PlayerLogic.Visual;
using Core.Scripts.Game.ScriptableObjects.Items;
using Fusion;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace Core.Scripts.Game.PlayerLogic.Combat
{
    public abstract class CombatNetworkBehaviour : CombatData
    {
        [Title("Network Behaviour", subtitle: "", TitleAlignments.Right), Networked, UnitySerializeField]
        protected internal int PlayerWeaponId { get; protected set; }

        private ChangeDetector _changeDetector;

        public override void Spawned()
        {
            base.Spawned();
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

            if (Object.HasStateAuthority)
            {
                Inventory.CurrentWeapon
                    .Where(w => w != null)
                    .Subscribe(SetWeaponInHand)
                    .AddTo(Disposables);
            }
        }
        
        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
        }

        public override void Render()
        {
            foreach (string change in
                     _changeDetector.DetectChanges(this,
                         out NetworkBehaviourBuffer previous,
                         out NetworkBehaviourBuffer current))
            {
                switch (change)
                {
                    case nameof(PlayerWeaponId):
                        WeaponChanged();
                        break;
                }
            }
        }

        private void WeaponChanged()
        {
            for (int i = 0; i < weaponData.Length; i++)
            {
                if (weaponData[i].weaponConfig.id != PlayerWeaponId) continue;
                EnableWeapon(weaponData[i]);
                ChangeAnimatorController(weaponData[i].weaponConfig.weaponAnimations);
                break;
            }
        }

        private void ChangeAnimatorController(AnimatorOverrideController controller)
        {
            // baseAnimation.OverrideAnimatorController(controller);
        }

        private void EnableWeapon(WeaponData data)
        {
            for (int i = 0; i < data.visuals.Length; i++)
            {
                data.visuals[i].prefab.SetActive(true);
            }
        }

        private void SetWeaponInHand(Weapon weapon)
        {
            if (Object.HasStateAuthority)
            {
                Debug.Log($"[CombatNetworkBehaviour] SetWeaponInHand {weapon.id}");
                PlayerWeaponId = weapon.id;
            }
        }
    }
}
using Core.Scripts.Game.Player.Inventory;
using Core.Scripts.Game.Player.Movement;
using Core.Scripts.Game.Player.VisualData;
using Fusion;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game.Player.Combat
{
    public abstract class CombatData : NetworkBehaviour
    {
        [Title("Combat Data", "", TitleAlignments.Right), SerializeField]
        protected PlayerBaseAnimation baseAnimation;
        [SerializeField, TableList] protected WeaponData[] weaponData;

        protected IPlayerInventory Inventory;
        protected CompositeDisposable Disposables;
        
        [Inject]
        public void Constructor(IPlayerInventory inventory)
        {
            Inventory = inventory;
        }

        public override void Spawned()
        {
            base.Spawned();
            Disposables = new CompositeDisposable();
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            Disposables.Clear();
        }
    }
}
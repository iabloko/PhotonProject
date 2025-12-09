using Core.Scripts.Game.PlayerLogic.Inventory;
using Core.Scripts.Game.PlayerLogic.NetworkInput;
using Core.Scripts.Game.PlayerLogic.Visual;
using Fusion;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game.PlayerLogic.Combat
{
    public abstract class CombatData : NetworkBehaviour
    {
        [SerializeField] protected PlayerInput input;
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
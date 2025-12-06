using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Views.Base;
using Core.Scripts.Game.Player.Inventory;
using Core.Scripts.Game.ScriptableObjects.Items;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Views
{
    [System.Serializable]
    public struct InventorySlot
    {
        public int id;
        public Image icon;
        public GameObject parent;
    }
    
    public sealed class GameMenuUIGamePlayView : GameMenuUIViewBase
    {
        [SerializeField] private InventorySlot[] inventorySlots;
        private IPlayerInventory _inventory;
        
        protected override void OnBeforeOpen()
        {
            for (int i = 0; i < inventorySlots.Length; i++)
                inventorySlots[i].parent.SetActive(false);
        }

        public void SetInventoryLogic(IPlayerInventory inventory)
        {
            _inventory = inventory;
            _inventory.CurrentWeapon
                .Where(w => w != null)
                .Subscribe(SetWeaponIcon)
                .AddTo(this);
        }

        private void SetWeaponIcon(Weapon weapon)
        {
            if (weapon == null)
            {
                Debug.LogError($"SetWeaponIcon weapon is null");
                return;
            }
            
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].id != weapon.id) continue;
                inventorySlots[i].parent.SetActive(true);
                inventorySlots[i].icon.sprite = weapon.icon;
            }
        }

        protected override void OnBeforeClosed()
        {
        }
    }
}
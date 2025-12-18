using Core.Scripts.Game.GamePlay.UsableItems;
using Core.Scripts.Game.Infrastructure.Services.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game.GamePlay.PickupableLogic
{
    public sealed class ItemPicker : MonoBehaviour
    {
        [SerializeField, InlineEditor, HideLabel] 
        internal Weapon pickUpItem;
        private const string PLAYER = "Player";
        private IInventory _inventory;

        [Inject]
        public void Constructor(IInventory inventory) => _inventory = inventory;

        private void OnTriggerEnter(Collider other)
        {
            Transform parent = other.transform.parent;
            
            if (parent != null && parent.CompareTag(PLAYER))
            {
                _inventory.PickWeapon(pickUpItem);
            }
        }
    }
}
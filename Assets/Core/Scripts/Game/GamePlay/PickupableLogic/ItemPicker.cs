using Core.Scripts.Game.PlayerLogic.Inventory;
using Core.Scripts.Game.ScriptableObjects.Items;
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
        private IPlayerInventory _inventory;

        [Inject]
        public void Constructor(IPlayerInventory inventory) => _inventory = inventory;

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
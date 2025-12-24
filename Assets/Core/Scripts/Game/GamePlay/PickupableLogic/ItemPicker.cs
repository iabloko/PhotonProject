using Core.Scripts.Game.Constants;
using Core.Scripts.Game.GamePlay.UsableItems;
using Core.Scripts.Game.PlayerLogic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.GamePlay.PickupableLogic
{
    public sealed class ItemPicker : MonoBehaviour
    {
        [SerializeField, InlineEditor, HideLabel] 
        internal Weapon pickUpItem;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(GameConstants.LOCAL_PLAYER)) return;
            
            GameObject parent = other.transform.parent.gameObject;
            parent.TryGetComponent(out IItemPickUpHandler handler);
            handler?.TryPickUp(pickUpItem);
        }
    }
}
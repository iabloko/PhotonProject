using Core.Scripts.Game.ScriptableObjects.Items;
using UnityEngine;

namespace Core.Scripts.Game.GamePlay.PickupableLogic
{
    public interface IPickupable
    {
    }

    public sealed class WeaponPickupable : MonoBehaviour, IPickupable
    {
        [SerializeField] internal Item pickUpItem;
        private const string PLAYER = "Player";

        private void OnTriggerEnter(Collider other)
        {
            Transform parent = other.transform.parent;
            
            if (parent != null && parent.CompareTag(PLAYER))
            {
                Debug.Log($"[WeaponPickupable] OnTriggerEnter try to pick up item: {pickUpItem.name}");
            }
        }
    }
}
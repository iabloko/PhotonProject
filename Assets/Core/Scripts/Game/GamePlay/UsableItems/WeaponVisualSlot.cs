using UnityEngine;

namespace Core.Scripts.Game.GamePlay.UsableItems
{
    [System.Serializable]
    public struct WeaponVisualSlot
    {
        public EquipmentSlot slot;
        public GameObject prefab;
    }
}
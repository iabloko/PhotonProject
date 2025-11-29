using UnityEngine;

namespace Core.Scripts.Game.ScriptableObjects.Weapon
{
    [System.Serializable]
    public struct EquippableWeapon
    {
        public GameObject prefab;
    }
    
    [CreateAssetMenu(menuName = "Settings/Configs/WeaponConfig", fileName = "WeaponConfig")]
    public sealed class Weapons : ScriptableObject
    {
        public EquippableWeapon[] equippableWeapons;
    }
}
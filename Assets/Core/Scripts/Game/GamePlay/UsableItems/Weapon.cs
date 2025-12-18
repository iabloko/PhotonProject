using UnityEngine;

namespace Core.Scripts.Game.GamePlay.UsableItems
{
    [CreateAssetMenu(menuName = "Game/Weapons/Weapon", fileName = "Weapon")]
    public sealed class Weapon : Item
    {
        public Sprite icon;
        public AnimatorOverrideController weaponAnimations;
        public override string ToString() => $"Weapon {id} |  {name}"; 
    }
}
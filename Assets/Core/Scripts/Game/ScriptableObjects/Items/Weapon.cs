using UnityEngine;

namespace Core.Scripts.Game.ScriptableObjects.Items
{
    public abstract class Weapon : Item
    {
        [SerializeField] internal string addressableLink;
    }
}
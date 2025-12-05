using UnityEngine;

namespace Core.Scripts.Game.ScriptableObjects.Items
{
    public abstract class Item : ScriptableObject
    {
        [SerializeField] internal string id;
        [SerializeField] internal string itemName;
    }
}
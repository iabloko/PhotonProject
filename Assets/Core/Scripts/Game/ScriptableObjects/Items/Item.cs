using UnityEngine;

namespace Core.Scripts.Game.ScriptableObjects.Items
{
    public abstract class Item : ScriptableObject
    {
        [SerializeField] public int id;
        [SerializeField] public string itemName;
        [SerializeField] public string addressableLink;
    }
}
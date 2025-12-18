using UnityEngine;

namespace Core.Scripts.Game.GamePlay.UsableItems
{
    public abstract class Item : ScriptableObject
    {
        [SerializeField] public int id;
        [SerializeField] public string itemName;
        [SerializeField] public string addressableLink;
    }
}

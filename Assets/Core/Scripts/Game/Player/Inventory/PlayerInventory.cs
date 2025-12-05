using Core.Scripts.Game.ScriptableObjects.Items;

namespace Core.Scripts.Game.Player.Inventory
{
    public interface IPlayerInventory
    {
        public void PickWeapon(Weapon weapon);
        public void RemoveWeapon(Weapon weapon);
    }

    public sealed class PlayerInventory : IPlayerInventory
    {
        void IPlayerInventory.PickWeapon(Weapon weapon)
        {
        }

        void IPlayerInventory.RemoveWeapon(Weapon weapon)
        {
        }
    }
}
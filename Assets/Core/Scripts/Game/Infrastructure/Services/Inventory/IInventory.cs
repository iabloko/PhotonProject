using System;
using System.Collections.Generic;
using Core.Scripts.Game.GamePlay.UsableItems;
using UniRx;

namespace Core.Scripts.Game.Infrastructure.Services.Inventory
{
    public interface IInventory
    {
        public IReadOnlyCollection<Item> Weapons { get; }
        public void PickWeapon(Weapon weapon);
        public void RemoveWeapon(Weapon weapon);
        
        IReadOnlyReactiveProperty<Weapon> CurrentWeapon { get; }
        IObservable<Weapon> OnWeaponPicked { get; }
    }
}
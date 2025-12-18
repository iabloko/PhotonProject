using System;
using System.Collections.Generic;
using Core.Scripts.Game.GamePlay.UsableItems;
using UniRx;

namespace Core.Scripts.Game.Infrastructure.Services.Inventory
{
    public sealed class PlayerInventory : IInventory
    {
        IReadOnlyReactiveProperty<Weapon> IInventory.CurrentWeapon => _currentWeapon;
        IObservable<Weapon> IInventory.OnWeaponPicked => _onWeaponPicked;
        
        private readonly ReactiveProperty<Weapon> _currentWeapon;
        private readonly Subject<Weapon> _onWeaponPicked;

        public PlayerInventory()
        {
            Weapons = new HashSet<Item>();
            _currentWeapon = new ReactiveProperty<Weapon>();
            _onWeaponPicked = new Subject<Weapon>();
        }

        public HashSet<Item> Weapons { get; }

        void IInventory.PickWeapon(Weapon weapon)
        {
            if (weapon == null) return;
            
            Weapons.Add(weapon);
            
            _onWeaponPicked.OnNext(weapon);
            _currentWeapon.Value = weapon;
        }

        void IInventory.RemoveWeapon(Weapon weapon)
        {
            if (weapon == null) return;

            Weapons.Remove(weapon);

            if (_currentWeapon.Value == weapon)
            {
                _currentWeapon.Value = null;
            }
        }
    }
}
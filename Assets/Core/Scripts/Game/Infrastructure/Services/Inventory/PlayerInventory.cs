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
        IReadOnlyCollection<Item> IInventory.Weapons => _weapons;
        
        private readonly ReactiveProperty<Weapon> _currentWeapon;
        private readonly Subject<Weapon> _onWeaponPicked;
        private readonly HashSet<Item> _weapons;

        public PlayerInventory()
        {
            _weapons = new HashSet<Item>();
            _currentWeapon = new ReactiveProperty<Weapon>();
            _onWeaponPicked = new Subject<Weapon>();
        }

        void IInventory.PickWeapon(Weapon weapon)
        {
            if (weapon == null) return;
            
            _weapons.Add(weapon);
            
            _onWeaponPicked.OnNext(weapon);
            _currentWeapon.Value = weapon;
        }

        void IInventory.RemoveWeapon(Weapon weapon)
        {
            if (weapon == null) return;

            _weapons.Remove(weapon);

            if (_currentWeapon.Value == weapon)
            {
                _currentWeapon.Value = null;
            }
        }
    }
}
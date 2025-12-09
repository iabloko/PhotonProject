using System;
using System.Collections.Generic;
using Core.Scripts.Game.ScriptableObjects.Items;
using UniRx;
using UnityEngine;

namespace Core.Scripts.Game.PlayerLogic.Inventory
{
    public interface IPlayerInventory
    {
        public HashSet<Item> Weapons { get; }
        public void PickWeapon(Weapon weapon);
        public void RemoveWeapon(Weapon weapon);
        
        IReadOnlyReactiveProperty<Weapon> CurrentWeapon { get; }
        IObservable<Weapon> OnWeaponPicked { get; }
    }

    public sealed class PlayerInventory : IPlayerInventory
    {
        IReadOnlyReactiveProperty<Weapon> IPlayerInventory.CurrentWeapon => _currentWeapon;
        IObservable<Weapon> IPlayerInventory.OnWeaponPicked => _onWeaponPicked;
        
        private readonly ReactiveProperty<Weapon> _currentWeapon;
        private readonly Subject<Weapon> _onWeaponPicked;

        public PlayerInventory()
        {
            Weapons = new HashSet<Item>();
            _currentWeapon = new ReactiveProperty<Weapon>();
            _onWeaponPicked = new Subject<Weapon>();
        }

        public HashSet<Item> Weapons { get; }

        void IPlayerInventory.PickWeapon(Weapon weapon)
        {
            if (weapon == null) return;
            
            Debug.Log($"[IPlayerInventory] PickWeapon {weapon.ToString()}");

            Weapons.Add(weapon);
            
            _onWeaponPicked.OnNext(weapon);
            _currentWeapon.Value = weapon;
        }

        void IPlayerInventory.RemoveWeapon(Weapon weapon)
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
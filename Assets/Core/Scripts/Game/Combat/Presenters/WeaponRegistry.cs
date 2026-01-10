using System.Collections.Generic;
using Core.Scripts.Game.Combat.Events;
using UnityEngine;

namespace Core.Scripts.Game.Combat.Presenters
{
    /// <summary>
    /// Реестр оружия. Загружает конфиги из ScriptableObject или Resources.
    /// </summary>
    public sealed class WeaponRegistry : IWeaponRegistry
    {
        private readonly Dictionary<int, WeaponCombatConfig> _weapons;

        public WeaponRegistry()
        {
            _weapons = new Dictionary<int, WeaponCombatConfig>();
        }

        public WeaponRegistry(IEnumerable<WeaponCombatConfig> configs) : this()
        {
            foreach (WeaponCombatConfig config in configs)
            {
                Register(config);
            }
        }

        public void Register(WeaponCombatConfig config)
        {
            if (config == null) return;

            if (_weapons.ContainsKey(config.Id))
            {
                Debug.LogWarning($"[WeaponRegistry] Weapon with ID {config.Id} already registered. Overwriting.");
            }

            _weapons[config.Id] = config;
        }

        public void Unregister(int weaponId)
        {
            _weapons.Remove(weaponId);
        }

        public bool TryGetConfig(int weaponId, out WeaponCombatConfig config)
        {
            return _weapons.TryGetValue(weaponId, out config);
        }

        public IEnumerable<WeaponCombatConfig> GetAll()
        {
            return _weapons.Values;
        }
    }

    /// <summary>
    /// Реестр оружия, загружающий конфиги из Resources.
    /// </summary>
    public sealed class ResourcesWeaponRegistry : IWeaponRegistry
    {
        private readonly WeaponRegistry _registry;

        public ResourcesWeaponRegistry(string resourcesPath = "Weapons")
        {
            _registry = new WeaponRegistry();

            var configs = Resources.LoadAll<WeaponCombatConfig>(resourcesPath);
            foreach (WeaponCombatConfig config in configs)
            {
                _registry.Register(config);
            }

            Debug.Log($"[ResourcesWeaponRegistry] Loaded {configs.Length} weapon configs from '{resourcesPath}'");
        }

        public bool TryGetConfig(int weaponId, out WeaponCombatConfig config)
        {
            return _registry.TryGetConfig(weaponId, out config);
        }
    }
}

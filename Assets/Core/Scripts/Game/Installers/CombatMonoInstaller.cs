using Core.Scripts.Game.Combat.Events;
using Core.Scripts.Game.Combat.Presenters;
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game.Installers
{
    [CreateAssetMenu(fileName = "CombatInstaller", menuName = "Installers/CombatInstaller")]
    public sealed class CombatInstaller : ScriptableObjectInstaller<CombatInstaller>
    {
        [Header("Weapon Configs"), SerializeField]
        private WeaponCombatConfig[] _weaponConfigs;

        public override void InstallBindings()
        {
            WeaponRegistry registry = new(_weaponConfigs);
            Container.Bind<IWeaponRegistry>().FromInstance(registry).AsSingle().Lazy();
        }
    }
    
    // public sealed class CombatMonoInstaller : MonoInstaller
    // {
    //     [Header("Weapon Configs"), SerializeField]
    //     private WeaponCombatConfig[] _weaponConfigs;
    //
    //     public override void InstallBindings()
    //     {
    //         WeaponRegistry registry = new(_weaponConfigs);
    //         
    //         Container.Bind<IWeaponRegistry>()
    //             .FromInstance(registry)
    //             .AsSingle();
    //     }
    // }
    //
    // public sealed class ResourcesCombatInstaller : MonoInstaller
    // {
    //     [Header("Resources Path"), SerializeField]
    //     private string _weaponsPath = "Configs/Weapons";
    //
    //     public override void InstallBindings()
    //     {
    //         Container.Bind<IWeaponRegistry>()
    //             .FromInstance(new ResourcesWeaponRegistry(_weaponsPath))
    //             .AsSingle();
    //     }
    // }
}
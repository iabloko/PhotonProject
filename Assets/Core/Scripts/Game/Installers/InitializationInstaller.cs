using Core.Scripts.Game.Infrastructure.Bootstrapper;
using Core.Scripts.Game.Infrastructure.ProjectNetworking.Provider;
using Core.Scripts.Game.Infrastructure.ProjectNetworking.Service;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.Services.CinemachineService;
using Core.Scripts.Game.Infrastructure.Services.Input;
using Core.Scripts.Game.Infrastructure.Services.Inventory;
using Core.Scripts.Game.Infrastructure.Services.NickName;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States;
using UnityEngine;
using Zenject;
using AssetProvider = Core.Scripts.Game.Infrastructure.Services.AssetProviderService.AssetProvider;

namespace Core.Scripts.Game.Installers
{
    public sealed class InitializationInstaller : MonoInstaller
    {
        [SerializeField] private Transform networkObjectProvider;
        
        public override void InstallBindings()
        {
            BindCoreServices();
            BindGameStateMachine();
            BindBootstrap();
        }

        private void BindCoreServices()
        {
            Container.Bind<IAssetProvider>().To<AssetProvider>().AsSingle().NonLazy();
            Container.Bind<IProjectSettings>().To<ProjectSettings>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<NickNameFadeEffect>().AsSingle().NonLazy();
            Container.Bind<IKeyHandler>().To<StandardAloneKeyHandler>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<Cinemachine>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<ZenjectNetworkObjectProvider>()
                .FromNewComponentOn(networkObjectProvider.gameObject)
                .AsSingle()
                .NonLazy();
            
            Container.Bind<INetworkService>().To<NetworkService>().AsSingle().NonLazy();
        }
        
        private void BindGameStateMachine()
        {
            Container.Bind<BootStrapState>().AsSingle();
            Container.Bind<LoadLevelState>().AsSingle();
            Container.Bind<PhotonLobbyState>().AsSingle();
            Container.Bind<GamePlayState>().AsSingle();

            Container.BindFactory<GameStateMachine, BootStrapState, BootStrapState.Factory>().AsSingle();
            Container.BindFactory<GameStateMachine, LoadLevelState, LoadLevelState.Factory>().AsSingle();
            Container.BindFactory<GameStateMachine, PhotonLobbyState, PhotonLobbyState.Factory>().AsSingle();
            Container.BindFactory<GameStateMachine, GamePlayState, GamePlayState.Factory>().AsSingle();

            Container.Bind<GameStateMachine>().AsSingle().NonLazy();
        }
        
        private void BindBootstrap()
        {
            Container.BindInterfacesTo<GameBootstrapper>().AsSingle().NonLazy();
        }
    }
}
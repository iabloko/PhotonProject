using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain;
using Zenject;

namespace Core.Scripts.Game.Installers
{
    public sealed class InitializationInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindHelpers();
            BindMainGameUI();
            
            Container.Bind<GameStartup>().AsSingle().NonLazy();
        }

        private void BindHelpers()
        {
            Container.Bind<IAssetProvider>().To<AssetProvider>().AsSingle().NonLazy();
        }

        private void BindMainGameUI()
        {
            Container.Bind<MainGameUIStateMachine>().AsSingle().NonLazy();
        }
    }
}
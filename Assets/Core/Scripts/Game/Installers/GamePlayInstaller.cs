using Core.Scripts.Game.Infrastructure.Services.Inventory;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.States;
using Zenject;

namespace Core.Scripts.Game.Installers
{
    public sealed class GamePlayInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindGameServices();
            BindGameStateMachine();
        }

        private void BindGameServices()
        {
            Container.Bind<IInventory>().To<PlayerInventory>().AsSingle().NonLazy();
        }

        private void BindGameStateMachine()
        {
            Container.Bind<GameMenuUIDescriptionState>().AsSingle();
            Container.Bind<GameMenuUIGamePlayState>().AsSingle();

            Container.BindFactory<MainGameUIStateMachine, GameMenuUIDescriptionState, GameMenuUIDescriptionState.Factory>().AsSingle();
            Container.BindFactory<MainGameUIStateMachine, GameMenuUIGamePlayState, GameMenuUIGamePlayState.Factory>().AsSingle();

            Container.Bind<MainGameUIStateMachine>().AsSingle().NonLazy();
        }
    }
}
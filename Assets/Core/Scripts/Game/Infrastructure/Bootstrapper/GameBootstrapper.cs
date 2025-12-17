using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States;
using Zenject;

namespace Core.Scripts.Game.Infrastructure.Bootstrapper
{
    public sealed class GameBootstrapper : IInitializable
    {
        private readonly GameStateMachine _machine;

        [Inject]
        public GameBootstrapper(GameStateMachine machine) => _machine = machine;
        
        public void Initialize() => _machine.Enter<BootStrapState>();
    }
}
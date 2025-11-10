using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain;
using Zenject;
using GameConfig = Core.Scripts.Game.ScriptableObjects.Configs.GameConfig;

namespace Core.Scripts.Game
{
    public sealed class GameStartup
    {
        public readonly GameStateMachine GameGameStateMachine;
        
        [Inject]
        public GameStartup(
            GameConfig gameConfig, 
            IAssetProvider assetProvider, 
            MainGameUIStateMachine uiStateMachine)
        {
            GameGameStateMachine = new GameStateMachine(gameConfig, assetProvider, uiStateMachine);
        }

        public void Start()
        {
            GameGameStateMachine.Enter<BootStrapState>();
        }
    }
}
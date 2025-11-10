using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.StateMachines.DataInterfaces;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States.Base;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.States;
using Cysharp.Threading.Tasks;

namespace Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States
{
    public struct GamePlayStateData : IData
    {
    }

    internal sealed class GamePlayState : PayloadStateBase<GamePlayStateData>
    {
        private readonly IAssetProvider _assetProvider;
        private readonly MainGameUIStateMachine _uiStateMachine;
        
        public GamePlayState(GameStateMachine gameGameStateMachineBase, IAssetProvider assetProvider,
            MainGameUIStateMachine uiStateMachine) : base(gameGameStateMachineBase)
        {
            _assetProvider = assetProvider;
            _uiStateMachine = uiStateMachine;
        }

        public override string StateName => "GamePlayState";

        public override void Enter(GamePlayStateData data)
        {
            InitializeGame().Forget();
        }

        private async UniTaskVoid InitializeGame()
        {
            await _uiStateMachine.OpenOrEnter<GameMenuUIGamePlayState, GameMenuUIGamePlayData>(new GameMenuUIGamePlayData());
        }
        
        public override void Exit()
        {
        }
    }
}
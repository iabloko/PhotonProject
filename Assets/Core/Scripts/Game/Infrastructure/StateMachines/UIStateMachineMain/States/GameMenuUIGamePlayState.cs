using Core.Scripts.Game.GameHelpers;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.StateMachines.BaseData;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.States.Base;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Views;

namespace Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.States
{
    public struct GameMenuUIGamePlayData : IPayload
    {
    }

    public sealed class
        GameMenuUIGamePlayState : GameMenuUISimpleStateBase<GameMenuUIGamePlayView, GameMenuUIGamePlayData>
    {
        public GameMenuUIGamePlayState(MainGameUIStateMachine stateMachine, IAssetProvider assetProvider) : base(
            stateMachine, assetProvider)
        {
        }

        protected override void OnEntered()
        {
        }

        public override string StateName => "GameMenuUIGamePlayState";
        protected override string ViewPath => GameConstant.GAME_UI_GAME_PLAY_VIEW;
    }
}
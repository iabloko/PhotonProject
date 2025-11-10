using Core.Scripts.Game.GameHelpers;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.StateMachines.BaseData;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.States.Base;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Views;

namespace Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.States
{
    public struct GameMenuUIMainData : IPayload
    {
    }

    public sealed class
        GameMenuUICharacterState : GameMenuUISimpleStateBase<GameMenuUICharacterView, GameMenuUIMainData>
    {
        public GameMenuUICharacterState(MainGameUIStateMachine stateMachine, IAssetProvider assetProvider) : base(
            stateMachine, assetProvider)
        {
        }

        public override string StateName => "GameMenuUIMainState";
        protected override string ViewPath => GameConstant.GAME_UI_CHARACTER_VIEW;
    }
}
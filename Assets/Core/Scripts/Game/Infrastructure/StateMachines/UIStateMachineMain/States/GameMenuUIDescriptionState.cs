using Core.Scripts.Game.GameHelpers;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.States.Base;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Views;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using Zenject;

namespace Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.States
{
    public sealed class
        GameMenuUIDescriptionState : GameMenuUISimpleStateBase<GameMenuUIDescriptionView>
    {
        public GameMenuUIDescriptionState(MainGameUIStateMachine stateMachine, IAssetProvider assetProvider) : base(
            stateMachine, assetProvider)
        {
        }

        public override string StateName => "GameMenuUIDescriptionState";
        protected override string ViewPath => GameConstant.GAME_UI_DESCRIPTION_VIEW;

        protected override async UniTask OnStartEnter()
        {
            await base.OnStartEnter();
            _stateView.Setup();
        }

        protected override void OnEntered()
        {
            StateMachine.FadeLogic(1, 0).Forget();
            base.OnEntered();
        }
        
        [Preserve]
        public class Factory : PlaceholderFactory<MainGameUIStateMachine, GameMenuUIDescriptionState>
        {
            [Preserve]
            public Factory()
            {
            }
        }
    }
}
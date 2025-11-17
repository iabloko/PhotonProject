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
        GameMenuUIGamePlayState : GameMenuUISimpleStateBase<GameMenuUIGamePlayView>
    {
        public GameMenuUIGamePlayState(MainGameUIStateMachine stateMachine, IAssetProvider assetProvider) : base(
            stateMachine, assetProvider)
        {
        }

        public override string StateName => "GameMenuUIGamePlayState";
        protected override string ViewPath => GameConstant.GAME_UI_GAME_PLAY_VIEW;

        protected override void OnEntered()
        {
            _stateView.toMainView.onClick.RemoveAllListeners();
            _stateView.repeatGame.onClick.RemoveAllListeners();

            _stateView.toMainView.onClick.AddListener(() => ReturnToMainView().Forget());
            _stateView.repeatGame.onClick.AddListener(RepeatGame);
            
            StateMachine.FadeLogic(1, 0).Forget(); // fade out
            base.OnEntered();
        }

        private void RepeatGame()
        {
            _stateView.RepeatGame();
        }

        protected override void OnStartExit()
        {
            _stateView.toMainView.onClick.RemoveAllListeners();
            _stateView.repeatGame.onClick.RemoveAllListeners();
            
            base.OnStartExit();
        }

        private async UniTaskVoid ReturnToMainView()
        {
            await StateMachine.FadeLogic(0, 1);
        }
        
        [Preserve]
        public class Factory : PlaceholderFactory<MainGameUIStateMachine, GameMenuUIGamePlayState>
        {
            [Preserve]
            public Factory()
            {
            }
        }
    }
}
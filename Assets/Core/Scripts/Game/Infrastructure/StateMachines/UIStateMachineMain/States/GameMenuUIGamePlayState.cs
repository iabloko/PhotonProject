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
            // _stateView.AfkTimer.OnAfk += StartAfkLogic;

            // _stateView.SetUpGameMenuUIGamePlayView(_payload.GameLogic);
            // _payload.GameLogic.StartGamePlay();
            
            StateMachine.FadeLogic(1, 0).Forget(); // fade out
            base.OnEntered();
        }

        private void RepeatGame()
        {
            // _payload.GameLogic.CreateBanknotes();
            // _payload.GameLogic.StartGamePlay();
            _stateView.RepeatGame();
        }

        protected override void OnStartExit()
        {
            // _payload.GameLogic.ExitGamePlayState();
            
            _stateView.toMainView.onClick.RemoveAllListeners();
            _stateView.repeatGame.onClick.RemoveAllListeners();
            // _stateView.AfkTimer.OnAfk -= StartAfkLogic;
            
            base.OnStartExit();
        }

        private async UniTaskVoid ReturnToMainView()
        {
            await StateMachine.FadeLogic(0, 1); // fade in
            
            // await StateMachine.OpenOrEnter<GameMenuUIDescriptionState, GameMenuUIMainData>(
            //     new GameMenuUIMainData(_payload.GameLogic));
        }
        
        private void StartAfkLogic() => ReturnToMainView().Forget();
        
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
using Core.Scripts.Game.GameHelpers;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
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
        private readonly IProjectSettings _settings;

        public GameMenuUIDescriptionState(
            MainGameUIStateMachine stateMachine, 
            IAssetProvider assetProvider,
            IProjectSettings settings) : base(stateMachine, assetProvider)
        {
            _settings = settings;
        }

        public override string StateName => "GameMenuUIDescriptionState";
        protected override string ViewPath => GameConstant.GAME_UI_DESCRIPTION_VIEW;

        protected override async UniTask OnStartEnter()
        {
            await base.OnStartEnter();
            _stateView.StartGameButtonClicked += PrepareToStartGame;
        }

        protected override void OnStartExit()
        {
            base.OnStartExit();
            _stateView.StartGameButtonClicked -= PrepareToStartGame;
        }

        protected override void OnEntered()
        {
            StateMachine.FadeLogic(startValue: 1, endValue: 0).Forget();
            base.OnEntered();
        }

        private void PrepareToStartGame() => PrepareToStartGameAsync().Forget();

        private async UniTaskVoid PrepareToStartGameAsync()
        {
            await StateMachine.EnterAsync<GameMenuUIGamePlayState>();
            
            _settings.ChangeGamePauseStatus(false);
            _settings.SetCursor(false);
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
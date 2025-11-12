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
            // _stateView.startEasyGame.onClick.RemoveAllListeners();
            // _stateView.startEasyGame.onClick.AddListener(() => StartGameplay(GameDifficulty.Easy).Forget());

            // _stateView.startHardGame.onClick.RemoveAllListeners();
            // _stateView.startHardGame.onClick.AddListener(() => StartGameplay(GameDifficulty.Hard).Forget());

            StateMachine.FadeLogic(1, 0).Forget(); // fade out
            base.OnEntered();
        }

        protected override void OnStartExit()
        {
            // _stateView.startEasyGame.onClick.RemoveAllListeners();
            // _stateView.startHardGame.onClick.RemoveAllListeners();

            base.OnStartExit();
        }
        
        [Preserve]
        public class Factory : PlaceholderFactory<MainGameUIStateMachine, GameMenuUIDescriptionState>
        {
            [Preserve]
            public Factory()
            {
            }
        }

        // private HashSet<Banknote> CreateBanknotes()
        // {
        //     int rnd = UnityEngine.Random.Range(0, 2);
        //     _preCreateddifficulty = rnd == 0 ? GameDifficulty.Easy : GameDifficulty.Hard;
        //
        //     GameInformation.SetGameDifficulty(_preCreateddifficulty);
        //     return _payload.GameLogic.CreateBanknotes();
        // }
        //
        // private async UniTaskVoid StartGameplay(GameDifficulty difficulty)
        // {
        //     await StateMachine.FadeLogic(0, 1); // fade in
        //
        //     if (!_preCreateddifficulty.Equals(difficulty))
        //     {
        //         GameInformation.SetGameDifficulty(difficulty);
        //         _payload.GameLogic.CreateBanknotes();
        //     }
        //
        //     await StateMachine.OpenOrEnter<GameMenuUIGamePlayState, GameMenuUIGamePlayData>(
        //         new GameMenuUIGamePlayData(_payload.GameLogic));
        // }
    }
}
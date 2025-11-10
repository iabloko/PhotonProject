using System;
using System.Collections.Generic;
using Core.Scripts.Game.GameHelpers;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.StateMachines.BaseData;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.States;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Views;

namespace Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class MainGameUIStateMachine : UIAsyncStateMachineBase
    {
        private GameMenuUIParentView MainView;

        public MainGameUIStateMachine(IAssetProvider assetProvider) : base(assetProvider)
        {
            MainView = CreateMainView();
            MainView.Setup(this);

            CreateStates();
        }

        protected override string MainViewPath => GameConstant.GAME_UI_MAIN_VIEW;

        public override void SetUIStateView(UIAsyncPayloadView stateView) => MainView.SetChildView(stateView);

        protected override void SubscribeEvents()
        {
        }

        protected override void UnSubscribeEvents()
        {
        }

        private void CreateStates()
        {
            States = new Dictionary<Type, IAsyncExitState>
            {
                [typeof(GameMenuUICharacterState)] = new GameMenuUICharacterState(this, AssetProvider),
                [typeof(GameMenuUIGamePlayState)] = new GameMenuUIGamePlayState(this, AssetProvider),
            };
        }

        private GameMenuUIParentView CreateMainView() => AssetProvider
            .InstantiateObject<GameMenuUIParentView>(MainViewPath, null, true);
    }
}
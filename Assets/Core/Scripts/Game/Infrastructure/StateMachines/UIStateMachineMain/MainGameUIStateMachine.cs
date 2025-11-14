using System;
using System.Collections.Generic;
using System.Threading;
using Core.Scripts.Game.GameHelpers;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.StateMachines.BaseData;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.States;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Views;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class MainGameUIStateMachine : UIAsyncStateMachineBase, IDisposable
    {
        private GameMenuUIParentView _mainView;
        private CancellationTokenSource _cts;

        [Inject]
        public MainGameUIStateMachine(
            IAssetProvider assetProvider,
            GameMenuUIDescriptionState.Factory descriptionFactory,
            GameMenuUIGamePlayState.Factory gamePlayFactory) : base(assetProvider) =>
            SetUpStateMachine(descriptionFactory, gamePlayFactory).Forget();

        protected override string MainViewPath => 
            GameConstant.GAME_UI_MAIN_VIEW;

        void IDisposable.Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        private async UniTaskVoid SetUpStateMachine(
            GameMenuUIDescriptionState.Factory descriptionFactory,
            GameMenuUIGamePlayState.Factory gamePlayFactory)
        {
            _cts = new CancellationTokenSource();
            _mainView = await CreateMainView();
            _mainView.Setup(this);

            States = new Dictionary<Type, IAsyncExitState>
            {
                [typeof(GameMenuUIDescriptionState)] = descriptionFactory.Create(this),
                [typeof(GameMenuUIGamePlayState)] = gamePlayFactory.Create(this),
            };

            EnterAsync<GameMenuUIDescriptionState>().Forget();
        }

        public override void SetUIStateView(UIAsyncPayloadView stateView) => 
            _mainView.SetChildView(stateView);

        private UniTask<GameMenuUIParentView> CreateMainView() => 
            AssetProvider.InstantiateAsync<GameMenuUIParentView>(MainViewPath, _cts);

        public async UniTask FadeLogic(int startValue, int endValue, float delay = 0)
        {
            _mainView.fadeImage.raycastTarget = true;

            _mainView.fadeImage.color = new Color(_mainView.fadeImage.color.r, _mainView.fadeImage.color.g,
                _mainView.fadeImage.color.b, startValue);

            await _mainView.fadeImage.DOFade(endValue, _mainView.fadeConfig.duration)
                .SetEase(_mainView.fadeConfig.easeType).SetDelay(delay).ToUniTask();

            _mainView.fadeImage.raycastTarget = false;
        }
    }
}
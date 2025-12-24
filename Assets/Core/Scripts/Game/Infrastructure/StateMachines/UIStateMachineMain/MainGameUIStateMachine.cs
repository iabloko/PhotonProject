using System;
using System.Collections.Generic;
using System.Threading;
using Core.Scripts.Game.GameHelpers;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
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

        private readonly IProjectSettings _settings;
        private readonly GameMenuUIDescriptionState.Factory _descriptionFactory;
        private readonly GameMenuUIGamePlayState.Factory _gamePlayFactory;

        [Inject]
        public MainGameUIStateMachine(
            IAssetProvider assetProvider,
            IProjectSettings projectSettings,
            GameMenuUIDescriptionState.Factory descriptionFactory,
            GameMenuUIGamePlayState.Factory gamePlayFactory) : base(assetProvider)
        {
            _gamePlayFactory = gamePlayFactory;
            _descriptionFactory = descriptionFactory;
            _settings = projectSettings;
        }

        protected override string MainViewPath => GameConstant.GAME_UI_MAIN_VIEW;

        public async UniTaskVoid SetUpStateMachine()
        {
            _settings.ChangeGamePauseStatus(true);
            _settings.SetCursor(true);
            
            _cts = new CancellationTokenSource();
            _mainView = await CreateMainView();
            _mainView.Setup();

            States = new Dictionary<Type, IAsyncExitState>
            {
                [typeof(GameMenuUIDescriptionState)] = _descriptionFactory.Create(this),
                [typeof(GameMenuUIGamePlayState)] = _gamePlayFactory.Create(this),
            };

            EnterAsync<GameMenuUIDescriptionState>().Forget();
        }

        void IDisposable.Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
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
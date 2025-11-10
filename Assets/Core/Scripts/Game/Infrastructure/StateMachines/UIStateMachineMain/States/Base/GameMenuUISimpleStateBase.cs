using System.Threading;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.StateMachines.BaseData;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Views.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.States.Base
{
    public abstract class GameMenuUISimpleStateBase<TView, TPayload> : GameMenuUIStateBase<TPayload>
        where TView : GameMenuUIViewBase
        where TPayload : IPayload
    {
        protected readonly IAssetProvider AssetProvider;
        protected TView _stateView;
        protected GameObject DataLoader;

        public override IAsyncStateView StateView => _stateView;

        protected abstract string ViewPath { get; }

        protected GameMenuUISimpleStateBase(MainGameUIStateMachine stateMachine, IAssetProvider assetProvider)
        {
            AssetProvider = assetProvider;
            StateMachine = stateMachine;
        }

        public sealed override async UniTask Enter(TPayload payload)
        {
            await OnStartEnter(payload);
            await base.Enter(payload);
            OnEntered();
        }

        public sealed override async UniTask Exit()
        {
            OnStartExit();
            await base.Exit();
            OnExited();
        }

        protected virtual async UniTask OnStartEnter(TPayload payload)
        {
            TokenSource = new CancellationTokenSource();
            await Setup();
        }

        protected virtual void OnEntered()
        {
        }

        protected virtual void OnStartExit()
        {
        }

        protected virtual void OnExited()
        {
            TokenSource?.Cancel();
            TokenSource?.Dispose();
            Object.Destroy(_stateView.gameObject);
        }

        protected virtual async UniTask Setup()
        {
            await CreateView();
            StateMachine.SetUIStateView(_stateView);
            _stateView.Setup();
        }

        private async UniTask CreateView() =>
            _stateView = await AssetProvider.InstantiateAsync<TView>(ViewPath, TokenSource);
    }
}
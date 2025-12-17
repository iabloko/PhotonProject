using System;
using System.Threading;
using Core.Scripts.Game.Infrastructure.ProjectNetworking.MatchmakingAdapter;
using Core.Scripts.Game.Infrastructure.ProjectNetworking.Service;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;
using Zenject;

namespace Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States
{
    public sealed class PhotonLobbyState : StateBase
    {
        private readonly IAssetProvider _assetProvider;
        private readonly INetworkService _networkService;

        public PhotonLobbyState(GameStateMachine gsm, 
            IAssetProvider assetProvider, INetworkService networkService) : base(gsm)
        {
            _networkService = networkService;
            _assetProvider = assetProvider;
        }

        public override string StateName => "PhotonLobbyState";
        
        public override void Enter()
        {
            StartMatchMakingLogic().Forget();
        }
        
        private async UniTaskVoid StartMatchMakingLogic()
        {
            using CancellationTokenSource tokenSource = new();

            try
            {
                MatchmakingUIAdapter uiAdapter = await _assetProvider.InstantiateAsync<MatchmakingUIAdapter>(
                    AssetPaths.MATCHMAKING_ADAPTER, tokenSource, dontDestroy: false);
                uiAdapter.Init(_networkService, _assetProvider, GameStateMachine);
            }
            catch (OperationCanceledException exception)
            {
                Debug.LogError($"MatchmakingUIAdapter creation was cancelled {exception.Message}");
            }
            
            await _networkService.Connect();
        }

        public override void Exit()
        {
        }
        
        [Preserve]
        public class Factory : PlaceholderFactory<GameStateMachine, PhotonLobbyState>
        {
            [Preserve]
            public Factory()
            {
            }
        }
    }
}
using System.Threading;
using Core.Scripts.Game.Infrastructure.ProjectNetworking.Provider;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.ProjectNetworking.Service
{
    public sealed class NetworkSessionHelper
    {
        private readonly IAssetProvider _assetProvider;
        
        private NetworkSceneManagerDefault _sceneManagerDefault;
        private CancellationTokenSource _connectCts;

        public string MegamodLobbyName => "MegamodLobby";

        public NetworkSessionHelper(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public void CreateCancellationTokenSource()
        {
            _connectCts?.Cancel();
            _connectCts = new CancellationTokenSource();
        }
        
        public StartGameArgs CreateStartGameArgs(string sessionName, ZenjectNetworkObjectProvider customObjectProvider)
        {
            if (_sceneManagerDefault == null)
            {
                _sceneManagerDefault = _assetProvider.InstantiateObject<NetworkSceneManagerDefault>(
                    AssetPaths.NETWORK_SCENE_MANAGER, dontDestroy: true);
            }

            StartGameArgs startArgs = new()
            {
                GameMode = GameMode.Shared,
                SessionName = sessionName,
                ObjectProvider = customObjectProvider,
                SceneManager = _sceneManagerDefault,
                PlayerCount = 4,
                Scene = SceneRef.FromIndex(2),
                OnGameStarted = _ => { Debug.Log("INetworkService Game Started"); },
                CustomLobbyName = MegamodLobbyName,
                StartGameCancellationToken = _connectCts.Token
            };
            Debug.LogWarning($"INetworkService Connected with StartGameArgs: {startArgs.ToString()}");
            return startArgs;
        }
    }
}
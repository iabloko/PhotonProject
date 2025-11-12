using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.AssetProviderService
{
    public interface IAssetProvider
    {
        void ReleaseObject(Object obj);
        void ReleaseInstance(GameObject obj);
        
        public T InstantiateObject<T>(string path, Transform parent = null, bool dontDestroy = false,
            bool instantiateInWorldSpace = true) where T : MonoBehaviour;
        
        public UniTask<GameObject> InstantiateObjectAsync(string path, CancellationTokenSource cts,
            Transform parent = null, bool dontDestroy = false, bool instantiateInWorldSpace = true,
            bool trackHandle = true);

        public UniTask<T> InstantiateAsync<T>(string path, CancellationTokenSource cts,
            Transform parent = null, bool dontDestroy = false, bool instantiateInWorldSpace = true,
            bool trackHandle = true) where T : MonoBehaviour;
    }
}
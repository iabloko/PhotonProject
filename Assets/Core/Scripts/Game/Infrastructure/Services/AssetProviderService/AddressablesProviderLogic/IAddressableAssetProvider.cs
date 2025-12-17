using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Scripts.Game.Infrastructure.Services.AssetProviderService.AddressablesProviderLogic
{
    public interface IAssetReference
    {
        AsyncOperationHandle Handle { get; }
        Object Asset { get; }
    }
    
    public struct AssetReference<T> : IAssetReference where T : Object
    {
        public AsyncOperationHandle<T> Handle;
        AsyncOperationHandle IAssetReference.Handle => Handle;
        Object IAssetReference.Asset =>
            Handle.IsValid() && Handle is { IsDone: true, Status: AsyncOperationStatus.Succeeded }
                ? Handle.Result
                : null;

        public T TypedAsset => Handle.Result!;
    }
    
    public interface IAddressableAssetProvider
    {
        public void Release(Object instance);
        public void ReleaseInstance(GameObject gameObject);
        
        public UniTask UnLoadAsset<T>(string path);

        public UniTask<GameObject> InstantiateAsync(string path, Transform parent,
            bool instantiateInWorldSpace, bool trackHandle, CancellationTokenSource cts);

        public UniTask<IAssetReference> LoadAsync<T>
            (string address, CancellationToken token = default) where T : Object;
    }
}
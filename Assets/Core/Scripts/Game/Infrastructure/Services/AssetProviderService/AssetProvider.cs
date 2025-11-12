using System;
using System.Threading;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService.AddressablesProviderLogic;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService.ResourceProviderLogic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using Object = UnityEngine.Object;

namespace Core.Scripts.Game.Infrastructure.Services.AssetProviderService
{
    public sealed class AssetProvider : IAssetProvider
    {
        private readonly IAddressableAssetProvider _addressables;
        private readonly IResourceProvider _resources;

        [Inject]
        public AssetProvider()
        {
            _addressables = new AddressableAssetProvider();
            _resources = new ResourceProvider();
        }

        #region ASYNCHRONOUSLY

        async UniTask<T> IAssetProvider.InstantiateAsync<T>(string path,
            CancellationTokenSource cts, Transform parent, bool dontDestroy,
            bool instantiateInWorldSpace, bool trackHandle)
        {
            if (cts == null) throw new ArgumentNullException(nameof(cts));

            GameObject createdObject =
                await _addressables.InstantiateAsync(path, parent, instantiateInWorldSpace, trackHandle, cts);

            if (createdObject == null) return null;

            T component = createdObject.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError(
                    $"[AssetProvider] InstantiateAsync<{typeof(T).Name}>: prefab at '{path}' does not contain requested component. Releasing instance.");
                Addressables.ReleaseInstance(createdObject);
                return null;
            }

            if (dontDestroy)
                Object.DontDestroyOnLoad(createdObject);

            return component;
        }

        async UniTask<GameObject> IAssetProvider.InstantiateObjectAsync(string path,
            CancellationTokenSource cts, Transform parent, bool dontDestroy,
            bool instantiateInWorldSpace, bool trackHandle)
        {
            if (cts == null) throw new ArgumentNullException(nameof(cts));

            GameObject createdObject =
                await _addressables.InstantiateAsync(path, parent, instantiateInWorldSpace, trackHandle, cts);

            if (createdObject == null) return null;
            if (dontDestroy) Object.DontDestroyOnLoad(createdObject);

            return createdObject;
        }

        #endregion

        #region SYNCHRONOUSLY

        T IAssetProvider.InstantiateObject<T>(string path, Transform parent, bool dontDestroy,
            bool instantiateInWorldSpace)
        {
            T instance = _resources.InstantiateComponent<T>(path, parent, instantiateInWorldSpace);
            
            if (instance == null) return null;
            if (dontDestroy) Object.DontDestroyOnLoad(instance);

            return instance;
        }

        #endregion

        #region RELEASE

        void IAssetProvider.ReleaseObject(Object obj)
        {
            if (obj == null)
                return;

            Addressables.Release(obj);
        }

        void IAssetProvider.ReleaseInstance(GameObject obj)
        {
            if (obj == null)
                return;

            Addressables.ReleaseInstance(obj);
        }

        #endregion
    }
}
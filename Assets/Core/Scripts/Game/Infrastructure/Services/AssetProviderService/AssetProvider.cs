using System;
using System.Threading;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService.AddressablesProviderLogic;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService.ResourceProviderLogic;
using Cysharp.Threading.Tasks;
using Sandbox.Project.Scripts.Helpers.BetterSpaceStringHelper;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Scripts.Game.Infrastructure.Services.AssetProviderService
{
    // ReSharper disable once ConvertConstructorToMemberInitializers
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class AssetProvider : IAssetProvider
    {
        private readonly IAddressableAssetProvider _addressables;
        private readonly IResourceProvider _resources;

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

            createdObject.TryGetComponent(out T component);
            if (component == null)
            {
                ReleaseInstance(createdObject);
                Debug.LogError(
                    $"[AssetProvider] InstantiateAsync<{typeof(T).Name}>: prefab at '{path}' does not contain requested component. Releasing instance.");
                return null;
            }

            if (dontDestroy)
                Object.DontDestroyOnLoad(createdObject);

            createdObject.transform.name = path.CleanAssetName();
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

            createdObject.transform.name = path.CleanAssetName();
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

            instance.transform.name = path.CleanAssetName();
            return instance;
        }

        #endregion

        #region RELEASE

        public void ReleaseObject(Object obj)
        {
            if (obj == null) return;
            _addressables.Release(obj);
        }

        public void ReleaseInstance(GameObject obj)
        {
            if (obj == null) return;
            _addressables.ReleaseInstance(obj);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Core.Scripts.Game.Infrastructure.Services.AssetProviderService.AddressablesProviderLogic
{
    public sealed class AddressableAssetProvider : IAddressableAssetProvider
    {
        private const bool IS_LOGGING = true;

        private readonly Dictionary<string, AsyncOperationHandle> _assetHandles = new();
        private readonly Dictionary<string, int> _addressRefCounts = new();
        
        async UniTask<GameObject> IAddressableAssetProvider.InstantiateAsync(string path, Transform parent,
            bool instantiateInWorldSpace, bool trackHandle, CancellationTokenSource cts)
        {
            GameObject createdObject = null;
            Log<GameObject>(path);

            try
            {
                createdObject =
                    (await Addressables.InstantiateAsync(path, parent, instantiateInWorldSpace, trackHandle)
                        .WithCancellation(cts.Token)).gameObject;
            }
            catch (Exception e)
            {
                Debug.LogError($"InstantiateAsync: IMPOSSIBLE TO LOAD {path}");
                Debug.LogError($"InstantiateAsync: load object {e.Message} \n" +
                               $"InstantiateAsync: {e.StackTrace}");
            }

            return createdObject;
        }

        async UniTask<IAssetReference> IAddressableAssetProvider.LoadAsync<T>(
            string address, CancellationToken token)
        {
            Log<T>(address);

            if (_assetHandles.TryGetValue(address, out AsyncOperationHandle existing))
            {
                Addressables.ResourceManager.Acquire(existing);
                _addressRefCounts[address] = _addressRefCounts.GetValueOrDefault(address, 0) + 1;

                return new AssetReference<T> { Handle = existing.Convert<T>() };
            }

            var handle = Addressables.LoadAssetAsync<T>(address);
            try
            {
                await handle.Task.AsUniTask().AttachExternalCancellation(token);

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _assetHandles[address] = handle;
                    _addressRefCounts[address] = 1;
                    return new AssetReference<T> { Handle = handle };
                }

                Debug.LogError($"Failed to load {typeof(T).Name} at {address}. Status: {handle.Status}");
                if (handle.IsValid()) Addressables.Release(handle);
                return null;
            }
            catch (OperationCanceledException)
            {
                if (handle.IsValid()) Addressables.Release(handle);
                return null;
            }
            catch (Exception ex)
            {
                if (handle.IsValid()) Addressables.Release(handle);
                Debug.LogException(ex);
                return null;
            }
        }

        async UniTask IAddressableAssetProvider.UnLoadAsset<T>(string address)
        {
            await UniTask.Yield();

            if (!_assetHandles.TryGetValue(address, out AsyncOperationHandle handle)) return;

            int left = _addressRefCounts.GetValueOrDefault(address, 1) - 1;
            _addressRefCounts[address] = left;

            if (left <= 0)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                    Log<T>($"Unload {address}");
                }

                _assetHandles.Remove(address);
                _addressRefCounts.Remove(address);
            }
            else
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                    Log<T>($"Unload {address}");
                }
            }
        }

        void IAddressableAssetProvider.Release(Object instance) => Addressables.Release(instance);

        private void Log<T>(string path)
        {
#if UNITY_EDITOR
            if (IS_LOGGING)
                Debug.Log($"<color=white>EDITOR LOG => {typeof(T).Name}: {path} </color>");
#endif
        }
    }
}
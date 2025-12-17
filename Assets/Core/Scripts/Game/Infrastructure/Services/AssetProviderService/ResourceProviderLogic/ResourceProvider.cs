using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Scripts.Game.Infrastructure.Services.AssetProviderService.ResourceProviderLogic
{
    public sealed class ResourceProvider : IResourceProvider
    {
        public T Load<T>(string path) where T : Object => 
            Resources.Load<T>(path);

        public Object Load(string path) => 
            Resources.Load(path);

        public async UniTask<T> LoadAsync<T>(string path, CancellationToken token = default) where T : Object
        {
            ResourceRequest request = Resources.LoadAsync<T>(path);

            while (!request.isDone)
            {
                if (token.IsCancellationRequested) return null;
                await UniTask.Yield();
            }

            return request.asset as T;
        }

        public T InstantiateComponent<T>(string path, Transform parent = null, bool dontDestroy = false,
            bool instantiateInWorldSpace = true) where T : Component
        {
            GameObject instance = Instantiate(path, parent, instantiateInWorldSpace);

            if (instance == null) return null;

            T component = instance.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"[ResourceProvider] Prefab at '{path}' does not contain component " +
                               $"{typeof(T).Name}. Destroying instance.");
                Object.Destroy(instance);
                return null;
            }

            if (dontDestroy) Object.DontDestroyOnLoad(instance);

            return component;
        }

        public GameObject Instantiate(string path, Transform parent = null, bool instantiateInWorldSpace = true)
        {
            GameObject prefab = Load<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogError($"[ResourceProvider] Can't load prefab at path: '{path}'");
                return null;
            }

            return Object.Instantiate(prefab, parent, instantiateInWorldSpace);
        }
    }
}

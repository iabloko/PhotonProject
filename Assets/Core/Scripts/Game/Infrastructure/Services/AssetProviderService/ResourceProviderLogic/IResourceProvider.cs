using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.AssetProviderService.ResourceProviderLogic
{
    public interface IResourceProvider
    {
        T Load<T>(string path) where T : Object;
        
        Object Load(string path);

        UniTask<T> LoadAsync<T>(string path, CancellationToken token = default) where T : Object;
        
        GameObject Instantiate(string path, Transform parent = null, bool instantiateInWorldSpace = true);
        
        T InstantiateComponent<T>(string path, Transform parent = null, bool dontDestroy = false,
            bool instantiateInWorldSpace = true) where T : Component;
    }
}
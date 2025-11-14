using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.GamePool.LoadHelper.Chains
{
    public sealed class ResourceFolderHandler : LoadHandler
    {
        private NetworkObject _cachedObject;

        protected override async UniTask<bool> CanHandle(string prefabId)
        {
            Debug.LogWarning($"[DefaultPool] {this} CanHandle {prefabId}");
            
            NetworkObject loadedObject = Resources.Load<NetworkObject>(prefabId);
            await UniTask.Yield();

            if (loadedObject == null)
            {
                //Debug.Log($"[DefaultPool] ResourceFolderHandler CanHandle {prefabId}: False");
                return false;
            }

            _cachedObject = loadedObject;
            // Debug.Log($"[DefaultPool] ResourceFolderHandler CanHandle {prefabId}: True");
            return true;
        }

        protected override async UniTask<Mesh> ExecuteLogic(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}
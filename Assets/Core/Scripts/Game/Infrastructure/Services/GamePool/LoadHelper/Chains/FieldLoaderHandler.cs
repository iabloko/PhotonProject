using System;
using Cysharp.Threading.Tasks;
using Sandbox.Project.Scripts.Helpers.BetterSpaceStringHelper;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.Scripts.Game.Infrastructure.Services.GamePool.LoadHelper.Chains
{
    public sealed class FieldLoaderHandler : LoadHandler
    {
        private Mesh _cachedMesh;

        protected override async UniTask<bool> CanHandle(string prefabId)
        {
            try
            {
                Mesh handle = await Addressables.LoadAssetAsync<Mesh>(prefabId);
                bool canHandle = handle != null;
                if (!canHandle) return false;

                Debug.Log($"[LoadHandler] ServerLoadHandler");
                _cachedMesh = handle;
                handle.name = prefabId;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected override UniTask<Mesh> ExecuteLogic(string id)
        {
            _cachedMesh.name = id.RemoveChunkPrefix();
            return UniTask.FromResult(_cachedMesh);
        }
    }
}
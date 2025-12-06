using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.GamePool.LoadHelper.Chains
{
    public class ServerLoadHandler : LoadHandler
    {
        // private readonly VoxelMeshCreator _meshCreator;
        //
        // public ServerLoadHandler(VoxelMeshCreator meshCreator)
        // {
        //     _meshCreator = meshCreator;
        // }

        public ServerLoadHandler()
        {
        }

        protected override UniTask<bool> CanHandle(string prefabId)
        {
            return UniTask.FromResult(false);
            
            // if (string.IsNullOrEmpty(prefabId)) return UniTask.FromResult(false);
            //
            // const StringComparison cmp = StringComparison.Ordinal;
            //
            // bool canHandle = DataModels.DownloadItemPrefixes.Any(prefix =>
            //     !string.IsNullOrEmpty(prefix) && prefabId.StartsWith(prefix, cmp));
            //
            // Debug.LogWarning($"[LoadHandler] ServerLoadHandler: {canHandle}");
            //
            // return UniTask.FromResult(canHandle);
        }

        protected override async UniTask<Mesh> ExecuteLogic(string prefabId)
        {
            await UniTask.Yield();
            return null;
            
            // const string query = "?isPreview=false";
            //
            // string url = string.Concat(
            //     BetterSpaceConstants.PROD_API_URL,
            //     BetterSpaceConstants.LEVEL_SINGLE_PROPS_API,
            //     prefabId, query);
            //
            // (RequestResultData rrd, UserVoxModelDataShort data) result =
            //     await UnityWebRequestHelper.GetStructRequest<UserVoxModelDataShort>(url);
            //
            // Mesh createdMesh = _meshCreator.CreateOptimizeModel(
            //     result.data.ID,
            //     result.data.voxModelData.VoxelUnitData, false);
            //
            // createdMesh.name = result.data.ID;
            
            // return createdMesh;
        }
    }
}
using Core.Scripts.Game.Infrastructure.Services.GamePool.LoadHelper.Chains;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.GamePool.LoadHelper
{
    public sealed class ObjectLoadChainHandler
    {
        private readonly LoadHandler _loadHandler;

        // public ObjectLoadChainHandler(VoxelMeshCreator meshCreator)
        public ObjectLoadChainHandler()
        {
            FieldLoaderHandler fieldLoader = new();
            ServerLoadHandler serverLoadHandler = new();
            fieldLoader.SetNextHandler(serverLoadHandler);
            // ResourceFolderHandler resourceFolder = new();
            // AddressableHandler addressableHandler = new();

            // resourceFolder.SetNextHandler(addressableHandler);
            // addressableHandler.SetNextHandler(fieldLoader);

            _loadHandler = fieldLoader;
        }

        public async UniTask<Mesh> ExecuteChainLogic(string id) => await _loadHandler.Execute(id);
    }
}
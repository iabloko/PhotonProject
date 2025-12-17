using Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator.Creator.Base;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator.Creator
{
    public sealed class SimpleObjectCreator : ObjectCreatorBase
    {
        private readonly NetworkObject _prototype;

        public SimpleObjectCreator(NetworkObject prototype) => _prototype = prototype;

        public async UniTask Create(
            string prefabId, Vector3 position, Quaternion? rotation, Vector3? scale, Transform parent) =>
            await CreateSimpleObject(_prototype, prefabId, position, rotation, scale, parent);

        public async UniTask<NetworkObject> Duplicate(
            string prefabId, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent) =>
            await DuplicateSimpleObject(_prototype, prefabId, position, rotation, scale, parent);
    }
}
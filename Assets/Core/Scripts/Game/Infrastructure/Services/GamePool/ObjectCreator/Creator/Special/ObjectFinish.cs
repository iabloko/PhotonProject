using System;
using System.Threading;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator.Creator.Base;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator.Creator.Special
{
    public sealed class ObjectFinish : SpecialObjectCreator
    {
        public ObjectFinish(NetworkObject reference) : base(reference)
        {
        }

        public override async UniTask Create(string prefabId, Vector3 p, Quaternion r, Vector3 s, Transform parent, string properties,
            CancellationTokenSource cts)
        {
            if (!prefabId.Equals(AssetPaths.FINISH, StringComparison.Ordinal))
            {
                Debug.LogError(
                    $"ObjectFinish error, prefabId: {prefabId} : created object is {AssetPaths.FINISH}");
                return;
            }
            
            await UniTask.Yield();
            
            // await CreateSpecialObject<NetworkDraggable>(Reference, p, r, s, parent);
        }
    }
}
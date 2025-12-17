using System;
using System.Threading;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator.Creator.Base;
using Core.Scripts.Game.InteractionObjects;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator.Creator.Special
{
    public sealed class ObjectInvisibleLavaWall : SpecialObjectCreator
    {
        public ObjectInvisibleLavaWall(NetworkObject reference) : base(reference)
        {
        }

        public override async UniTask Create(string prefabId, Vector3 p, Quaternion r, Vector3 s, Transform parent, string properties,
            CancellationTokenSource cts)
        {
            if (!prefabId.Equals(AssetPaths.INVISIBLE_WALL_ID, StringComparison.Ordinal))
            {
                Debug.LogError(
                    $"ObjectInvisibleLavaWall, prefabId: {prefabId} : created object is {AssetPaths.INVISIBLE_WALL_ID}");
                return;
            }
            
            await CreateSpecialObject<InvisibleWall>(Reference, p, r, s, parent);
        }
    }
}
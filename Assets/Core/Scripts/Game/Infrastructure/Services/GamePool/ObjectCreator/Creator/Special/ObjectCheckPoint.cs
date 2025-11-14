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
    public sealed class ObjectCheckPoint : SpecialObjectCreator
    {
        public ObjectCheckPoint(NetworkObject reference) : base(reference)
        {
        }

        public override async UniTask Create(string prefabId, Vector3 p, Quaternion r, Vector3 s, Transform parent, string properties,
            CancellationTokenSource cts)
        {
            if (!prefabId.Equals(AssetPaths.CHECK_POINT_ID, StringComparison.Ordinal))
            {
                Debug.LogError(
                    $"ObjectCheckPoint error, prefabId: {prefabId} : created object is {AssetPaths.CHECK_POINT_ID}");
                return;
            }
            
            await CreateSpecialObject<CheckPointPlane>(Reference, p, r, s, parent);
        }
    }
}
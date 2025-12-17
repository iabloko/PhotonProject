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
    public sealed class ObjectInvisibleLavaLine : SpecialObjectCreator
    {
        public ObjectInvisibleLavaLine(NetworkObject reference) : base(reference)
        {
        }

        public override async UniTask Create(string prefabId, Vector3 p, Quaternion r, Vector3 s, Transform parent, string properties,
            CancellationTokenSource cts)
        {
            if (!prefabId.Equals(AssetPaths.INVISIBLE_LAVA_LINE_ID, StringComparison.Ordinal))
            {
                Debug.LogError(
                    $"ObjectInvisibleLavaLine error, prefabId: {prefabId} : created object is {AssetPaths.INVISIBLE_LAVA_LINE_ID}");
                return;
            }
            
            await CreateSpecialObject<InvisibleLavaLine>(Reference, p, r, s, parent);
        }
    }
}
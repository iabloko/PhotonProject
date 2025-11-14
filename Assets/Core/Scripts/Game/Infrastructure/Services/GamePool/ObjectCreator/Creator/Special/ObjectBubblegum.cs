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
    public sealed class ObjectBubblegum : SpecialObjectCreator
    {
        public ObjectBubblegum(NetworkObject reference) : base(reference)
        {
        }

        public override async UniTask Create(string prefabId, Vector3 p, Quaternion r, Vector3 s, Transform parent, string properties,
            CancellationTokenSource cts)
        {
            if (!prefabId.Equals(AssetPaths.BUBBLEGUM_ID, StringComparison.Ordinal))
            {
                Debug.LogError(
                    $"ObjectBubblegum error, prefabId: {prefabId} : created object is {AssetPaths.BUBBLEGUM_ID}");
                return;
            }
            
            await CreateSpecialObject<BubbleGumPlane>(Reference, p, r, s, parent);
        }
    }
}
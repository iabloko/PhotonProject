using System;
using System.Threading;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator.Creator.Base;
using Core.Scripts.Game.InteractionObjects;
using Cysharp.Threading.Tasks;
using Fusion;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator.Creator.Special
{
    public sealed class ObjectJumpPad : SpecialObjectCreator
    {
        public ObjectJumpPad(NetworkObject reference) : base(reference)
        {
        }

        public override async UniTask Create(string prefabId, Vector3 p, Quaternion r,
            Vector3 s, Transform parent, string properties, CancellationTokenSource cts)
        {
            if (!prefabId.Equals(AssetPaths.JUMP_PAD_ID, StringComparison.Ordinal))
            {
                Debug.LogError(
                    $"ObjectJumpPad error, prefabId: {prefabId} : created object is {AssetPaths.JUMP_PAD_ID}");
                return;
            }

            JumpPadPlane createdPrefab = await CreateSpecialObject<JumpPadPlane>(Reference, p, r, s, parent);

            JumpPadProperties jumpPadCtrlDefaultProperty = new() { JumpPadStrength = 100 };
            try
            {
                createdPrefab.Properties = JsonConvert.DeserializeObject<JumpPadProperties>(properties);
            }
            catch (Exception)
            {
                createdPrefab.Properties = jumpPadCtrlDefaultProperty;
            }
        }
    }
}
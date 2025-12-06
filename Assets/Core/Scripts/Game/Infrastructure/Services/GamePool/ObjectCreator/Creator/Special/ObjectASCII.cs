using System;
using System.Threading;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator.Creator.Base;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator.Creator.Special
{
    public sealed class ObjectAscii : SpecialObjectCreator
    {
        public ObjectAscii(NetworkObject reference) : base(reference)
        {
        }

        public override async UniTask Create(string prefabId, Vector3 p, Quaternion r,
            Vector3 s, Transform parent, string properties, CancellationTokenSource cts)
        {
            if (!prefabId.Equals(AssetPaths.ASCII, StringComparison.Ordinal))
            {
                Debug.LogError(
                    $"ObjectAsciiArt error: prefabId='{prefabId}', expected='{AssetPaths.ASCII}'");
                return;
            }
            
            await UniTask.Yield();

            // ObjectAsciiArt created = await CreateSpecialObject<ObjectAsciiArt>(Reference, p, r, s, parent);
            //
            // if (cts is { IsCancellationRequested: true }) return;
            //
            // AsciiProperties data = default;
            //
            // try
            // {
            //     data = JsonConvert.DeserializeObject<AsciiProperties>(properties);
            // }
            // catch (Exception ex)
            // {
            //     Debug.LogWarning(
            //         $"ASCII properties parse failed: {ex.Message}. Will use default smile. Raw='{properties}'");
            // }
            //
            // created.Properties = data;
        }
    }
}
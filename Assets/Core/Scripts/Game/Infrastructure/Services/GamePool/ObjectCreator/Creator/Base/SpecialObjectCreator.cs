using System.Threading;
using Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator.Creator.Special;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator.Creator.Base
{
    public abstract class SpecialObjectCreator : ObjectCreatorBase, IObjectCreator
    {
        protected readonly NetworkObject Reference;

        protected SpecialObjectCreator(NetworkObject reference) => Reference = reference;

        public abstract UniTask Create(string prefabId, Vector3 p, Quaternion r,
            Vector3 s, Transform parent, string properties, CancellationTokenSource cts);
        
        public virtual async UniTask<NetworkObject> Duplicate(
            Vector3 p, Quaternion r, Vector3 s, Transform parent, string properties, CancellationTokenSource cts) =>
            await DuplicateSpecialObject(Reference, p, r, s, parent);
    }
}
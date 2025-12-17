using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator.Creator.Special
{
    public interface IObjectCreator
    {
        UniTask Create(string prefabId, Vector3 p, Quaternion r, Vector3 s,
            Transform parent, string properties, CancellationTokenSource cts);

        UniTask<NetworkObject> Duplicate(Vector3 p, Quaternion r,
            Vector3 s, Transform parent, string properties, CancellationTokenSource cts);
    }
}
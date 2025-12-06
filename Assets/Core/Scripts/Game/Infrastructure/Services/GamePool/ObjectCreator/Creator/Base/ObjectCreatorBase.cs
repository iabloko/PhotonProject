using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator.Creator.Base
{
    public abstract class ObjectCreatorBase
    {
        #region SpecialObject

        protected async UniTask<T> CreateSpecialObject<T>(NetworkObject prototype,
            Vector3 position, Quaternion? rotation, Vector3? scale, Transform parent) where T : NetworkBehaviour
        {
            Vector3 propScale = scale ?? Vector3.one;
            Quaternion propRotation = rotation ?? Quaternion.identity;
            await UniTask.Yield();
            return null;
            // NetworkObject created = await PlayerInfo.CurrentRunner.SpawnAsync(
            //     prototype,
            //     position,
            //     propRotation,
            //     PlayerInfo.CurrentRunner.LocalPlayer,
            //     onBeforeSpawned: (runner, o) =>
            //     {
            //         o.transform.localScale = propScale;
            //         o.transform.SetParent(parent);
            //     });

            // return created.GetComponent<T>();
        }        
        
        public virtual async UniTask<NetworkObject> DuplicateSpecialObject(NetworkObject prototype,
            Vector3 position, Quaternion? rotation, Vector3? scale, Transform parent)
        {
            Vector3 propScale = scale ?? Vector3.one;
            Quaternion propRotation = rotation ?? Quaternion.identity;
            await UniTask.Yield();
            
            return null;
            // return await PlayerInfo.CurrentRunner.SpawnAsync(
            //     prototype,
            //     position,
            //     propRotation,
            //     PlayerInfo.CurrentRunner.LocalPlayer,
            //     onBeforeSpawned: (runner, o) =>
            //     {
            //         o.transform.localScale = propScale;
            //         o.transform.SetParent(parent);
            //     });
        }

        #endregion


        #region SimpleObject

        protected async UniTask<NetworkObject> DuplicateSimpleObject
        (NetworkObject prototype, string prefabId, Vector3 position,
            Quaternion? rotation, Vector3? scale, Transform parent)
        {
            Vector3 propScale = scale ?? Vector3.one;
            Quaternion propRotation = rotation ?? Quaternion.identity;
            await UniTask.Yield();
            
            return null;
            
            // NetworkObject created = await PlayerInfo.CurrentRunner.SpawnAsync(
            //     prototype,
            //     position,
            //     propRotation,
            //     PlayerInfo.CurrentRunner.LocalPlayer,
            //     onBeforeSpawned: (_, o) =>
            //     {
            //         o.GetComponent<NetworkPropVisual>().InitKeyImmediately(prefabId);
            //         o.transform.localScale = propScale;
            //         o.transform.SetParent(parent);
            //     });

            // return created;
        }

        protected async UniTask CreateSimpleObject(NetworkObject prototype,
            string prefabId, Vector3 position, Quaternion? rotation, Vector3? scale, Transform parent)
        {
            Vector3 propScale = scale ?? Vector3.one;
            Quaternion propRotation = rotation ?? Quaternion.identity;
            await UniTask.Yield();
            
            // await PlayerInfo.CurrentRunner.SpawnAsync(
            //     prototype,
            //     position,
            //     propRotation,
            //     PlayerInfo.CurrentRunner.LocalPlayer,
            //     onBeforeSpawned: (_, o) =>
            //     {
            //         o.GetComponent<NetworkPropVisual>().InitKeyImmediately(prefabId);
            //         o.transform.localScale = propScale;
            //         o.transform.SetParent(parent);
            //     });
        }

        #endregion
    }
}
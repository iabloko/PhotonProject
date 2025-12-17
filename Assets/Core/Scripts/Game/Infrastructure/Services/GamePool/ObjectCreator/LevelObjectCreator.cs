using System.Collections.Generic;
using System.Threading;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.Services.GamePool.GameObjectReference;
using Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator.Creator;
using Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator.Creator.Special;
using Core.Scripts.Game.InteractionObjects.Base;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator
{
    public sealed class LevelObjectCreator
    {
        private readonly SimpleObjectCreator _baseCreator;
        private readonly Dictionary<string, IObjectCreator> _levelObjects;
        private readonly Dictionary<string, Portal> _portals;
        private readonly List<ISpecialPlane> _specialPlanes;

        public LevelObjectCreator(ObjectReference reference)
        {
            _portals = new Dictionary<string, Portal>();
            _specialPlanes = new List<ISpecialPlane>();
            _baseCreator = new SimpleObjectCreator(reference.prototype);

            _levelObjects = new Dictionary<string, IObjectCreator>
            {
                { AssetPaths.FINISH, new ObjectFinish(reference.finish) },
                { AssetPaths.SPEED_BOOSTER_ID, new ObjectSpeedBooster(reference.speed) },
                { AssetPaths.LAVA_PLANE_ID, new ObjectLavaPlane(reference.lava) },
                { AssetPaths.JUMP_PAD_ID, new ObjectJumpPad(reference.jump) },
                { AssetPaths.CHECK_POINT_ID, new ObjectCheckPoint(reference.check) },
                { AssetPaths.BUBBLEGUM_ID, new ObjectBubblegum(reference.bubblegum) },
                { AssetPaths.BANANA_ID, new ObjectBanana(reference.banana) },
                { AssetPaths.ASCII, new ObjectAscii(reference.ascii) },
                { AssetPaths.PORTAL_PRIMARY_ID, new ObjectPrimaryPortal(reference.primaryPortal, ref _portals) },
                { AssetPaths.PORTAL_SECOND_ID, new ObjectSecondPortal(reference.secondaryPortal, ref _portals) },
                
                { AssetPaths.INVISIBLE_LAVA_PLANE_ID, new ObjectInvisibleLavaPlane(reference.lavaPlane) },
                { AssetPaths.INVISIBLE_LAVA_LINE_ID, new ObjectInvisibleLavaLine(reference.lavaLine) },
                { AssetPaths.INVISIBLE_WALL_ID, new ObjectInvisibleLavaWall(reference.lavaWall) },
            };
        }

        public async UniTask Create(string prefabId, Vector3 position, Quaternion rotation,
            Vector3 scale, Transform parent, string properties, CancellationTokenSource cts)
        {
            _levelObjects.TryGetValue(prefabId, out IObjectCreator objectCreator);

            if (objectCreator == null)
            {
                await _baseCreator.Create(prefabId, position, rotation, scale, parent);
            }
            else
            {
                await objectCreator.Create(prefabId, position, rotation, scale, parent, properties, cts);
            }
        }

        public async UniTask<NetworkObject> Duplicate(string prefabId, Vector3 position, Quaternion rotation,
            Vector3 scale, Transform parent, string properties, CancellationTokenSource cts)
        {
            _levelObjects.TryGetValue(prefabId, out IObjectCreator objectCreator);

            if (objectCreator == null)
            {
                return await _baseCreator.Duplicate(
                    prefabId, position, rotation, scale, parent);
            }
            else
            {
                return await objectCreator.Duplicate(position, rotation, scale, parent, properties, cts);
            }
        }

        public void SetUpSpecialsObjects()
        {
            foreach (ISpecialPlane plane in _specialPlanes)
            {
                plane.EnableAtStart();
            }
        }

        public void UpdatePortals()
        {
            _portals.TryGetValue(AssetPaths.PORTAL_PRIMARY_ID, out Portal primaryPortal);
            _portals.TryGetValue(AssetPaths.PORTAL_SECOND_ID, out Portal secondPortal);

            if (primaryPortal != null) primaryPortal.nextPortal = secondPortal;
            if (secondPortal != null) secondPortal.nextPortal = primaryPortal;
        }
    }
}
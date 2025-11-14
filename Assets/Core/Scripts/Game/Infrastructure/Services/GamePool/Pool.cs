using System;
using System.Collections.Generic;
using Core.Scripts.Game.Infrastructure.Services.GamePool.GameObjectReference;
using Core.Scripts.Game.Infrastructure.Services.GamePool.LoadHelper;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Core.Scripts.Game.Infrastructure.Services.GamePool
{
    public sealed class Pool : PoolBase, INetworkPool
    {
        private readonly ObjectLoadChainHandler _loadChainHandler;
        private readonly Dictionary<string, UniTaskCompletionSource<Mesh>> _inFlightLoads;

        bool INetworkPool.IsCached(string id) => ResourceCache.ContainsKey(id);
        int INetworkPool.GetBackPackCount() => BackPack.Count;

        [Inject]
        public Pool(ObjectReference reference) : base(reference)
        {
            _inFlightLoads = new Dictionary<string, UniTaskCompletionSource<Mesh>>();
            _loadChainHandler = new ObjectLoadChainHandler();

            CreateParent();
        }

        async UniTaskVoid INetworkPool.CreateLoadedItems(string[] lines)
        {
            CheckPlayer();
            int positionIndex = 0;

            Vector3 position = Player.position;

            for (int index = 0; index < lines.Length; index++)
            {
                string id = lines[index];

                Vector3 calculatedPosition =
                    new Vector3(position.x, position.y + 0.67f, position.z) + Player.forward * (3 + positionIndex * 3);

                await CreateLevelObject(id, calculatedPosition, Quaternion.identity, Vector3.one, string.Empty);

                positionIndex++;
            }
        }

        // async UniTaskVoid INetworkPool.CreateLoadedServer(RoomData roomData)
        // {
        //     for (int index = 0; index < roomData.models.Length; index++)
        //     {
        //         Prop p = roomData.models[index];
        //
        //         Vector3 position = new((float)p.x, (float)p.y, (float)p.z);
        //         Quaternion rotation = Quaternion.Euler((float)p.rotation_x, (float)p.rotation_y, (float)p.rotation_z);
        //         Vector3 scale = new((float)p.scale, (float)p.scale, (float)p.scale);
        //
        //         string id = p.id.Replace("16x16x-", "16x-");
        //
        //         await CreateLevelObject(id, position, rotation, scale, p.properties);
        //     }
        // }

        async UniTask<T> INetworkPool.DuplicateObject<T>(
            string id, Vector3 position, Quaternion? rotation, Vector3? scale, string properties)
        {
            Vector3 propScale = scale ?? Vector3.one;
            Quaternion propRotation = rotation ?? Quaternion.identity;

            NetworkObject createdObject = await DuplicateLevelObject(id, position, propRotation, propScale, properties);

            return createdObject.GetComponent<T>();
        }

        // public async UniTask AddToPool(UserVoxModelDataShort[] resultData)
        // {
        //     for (int i = 0; i < resultData.Length; i++)
        //     {
        //         bool cached = ResourceCache.ContainsKey(resultData[i].ID);
        //
        //         if (!cached)
        //         {
        //             Mesh mesh = MeshCreator.CreateOptimizeModel(resultData[i].ID,
        //                 resultData[i].voxModelData.VoxelUnitData, false);
        //
        //             mesh.name = resultData[i].ID;
        //             TryAdd(resultData[i].ID, mesh);
        //         }
        //
        //         if (i % 100 == 0) await UniTask.Yield();
        //     }
        // }

        public async UniTask<Mesh> LoadResource(string id)
        {
            if (ResourceCache.TryGetValue(id, out Mesh cached)) return cached;

            if (_inFlightLoads.TryGetValue(id, out var existingTcs))
            {
                return await existingTcs.Task;
            }

            var tcs = new UniTaskCompletionSource<Mesh>();
            _inFlightLoads[id] = tcs;

            _ = LoadAndSignal(id, tcs);

            return await tcs.Task;
        }

        private async UniTaskVoid LoadAndSignal(string id, UniTaskCompletionSource<Mesh> tcs)
        {
            try
            {
                Mesh mesh = await _loadChainHandler.ExecuteChainLogic(id);
                TryAdd(id, mesh);
                tcs.TrySetResult(mesh);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
            finally
            {
                _inFlightLoads.Remove(id);
            }
        }
    }

    public interface INetworkPool
    {
        public UniTask<Mesh> LoadResource(string id);

        public UniTask<T> DuplicateObject<T>(
            string id, Vector3 position, Quaternion? rotation, Vector3? scale, string properties) where T : Object;

        public int GetBackPackCount();

        public bool IsCached(string id);

        // public UniTask AddToPool(UserVoxModelDataShort[] resultData);
        public UniTaskVoid CreateLoadedItems(string[] lines);
        // public UniTaskVoid CreateLoadedServer(RoomData roomData);
    }

    public interface IRequiresInjection
    {
        public bool RequiresInjection { get; set; }
    }
}
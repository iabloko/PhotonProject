using System;
using System.Collections.Generic;
using System.Threading;
using Core.Scripts.Game.Infrastructure.Services.GamePool.GameObjectReference;
using Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator;
using Cysharp.Threading.Tasks;
using Fusion;
using Sandbox.Project.Scripts.Constants;
using UnityEngine;
using UnityEngine.Scripting;
using Zenject;

namespace Core.Scripts.Game.Infrastructure.Services.GamePool
{
    public enum BlockRuleType
    {
        Exact,
        StartsWith
    }
    
    [Preserve]
    public sealed class BlockedIdRule
    {
        public BlockRuleType RuleType { get; }
        public string Value { get; }

        public BlockedIdRule(BlockRuleType ruleType, string value)
        {
            RuleType = ruleType;
            Value = value;
        }
    }
    
    public abstract class PoolBase
    {
        protected Dictionary<string, Mesh> ResourceCache { get; }
        
        // protected readonly EnvironmentProfile Profile;
        // protected readonly VoxelMeshCreator MeshCreator;
        
        protected readonly List<string> BackPack;
        protected Transform Parent;
        protected Transform Player;
        
        private readonly List<BlockedIdRule> _blockedRules = new()
        {
            new BlockedIdRule(BlockRuleType.Exact, BlockedObjects.BALL_ID),
            new BlockedIdRule(BlockRuleType.Exact, BlockedObjects.CONTROLLED_CUBE_ID),
            new BlockedIdRule(BlockRuleType.Exact, BlockedObjects.DOOR_ID),
            new BlockedIdRule(BlockRuleType.Exact, BlockedObjects.KEY_ID),
            new BlockedIdRule(BlockRuleType.Exact, BlockedObjects.SIGN_ID),
            new BlockedIdRule(BlockRuleType.Exact, BlockedObjects.COIN_ID),
            new BlockedIdRule(BlockRuleType.Exact, BlockedObjects.LEADERBOARD),
            new BlockedIdRule(BlockRuleType.Exact, BlockedObjects.COPY),
            new BlockedIdRule(BlockRuleType.StartsWith, BlockedObjects.COPY),
        };
        
        private readonly LevelObjectCreator _levelObjectCreator;
        private readonly CancellationTokenSource _cts;

        [Inject]
        protected PoolBase(ObjectReference reference)
        {
            // Profile = profile;
            // MeshCreator = new VoxelMeshCreator(Profile.gamePlay.baseTexture);
            ResourceCache = new Dictionary<string, Mesh>();
            BackPack = new List<string>();
            
            _levelObjectCreator = new LevelObjectCreator(reference);
            _cts = new CancellationTokenSource();
        }

        protected void TryAdd(string id, Mesh value)
        {
            if (!ResourceCache.ContainsKey(id)) Add(id, value);
        }

        protected void CreateParent()
        {
            // string sessionName = PlayerInfo.CurrentRunner.SessionInfo.Name;
            string sessionName = "SESSION_X-11";
            Parent = new GameObject(sessionName)
            {
                transform =
                {
                    position = Vector3.zero,
                    rotation = Quaternion.identity,
                },
            }.transform;
        }

        protected void CheckPlayer()
        {
            if (Player != null) return;
            
            // if (PlayerInfo.CurrentRunner.TryGetPlayerObject(PlayerInfo.CurrentRunner.LocalPlayer, out NetworkObject obj))
            // {
            //     Player = obj.transform;
            //     Debug.Log($"[PoolLoad]: {Player}");
            // }
        }

        protected async UniTask CreateLevelObject(
            string prefabId, Vector3 position, Quaternion rotation, Vector3 scale, string properties)
        {
            if (IsBlockedObject(prefabId)) return;
            
            await _levelObjectCreator.Create(prefabId, position, rotation, scale, Parent, properties, _cts);
        }        
        
        protected async UniTask<NetworkObject> DuplicateLevelObject(
            string prefabId, Vector3 position, Quaternion rotation, Vector3 scale, string properties) =>
            await _levelObjectCreator.Duplicate(prefabId, position, rotation, scale, Parent, properties, _cts);

        private void Add(string key, Mesh value)
        {
            if (value == null)
            {
                Debug.LogError($"[DefaultPool] error try to add empty mesh");
                return;
            }

            ResourceCache.TryAdd(key, value);
        }
        
        private bool IsBlockedObject(string prefabId)
        {
            foreach (BlockedIdRule rule in _blockedRules)
            {
                switch (rule.RuleType)
                {
                    case BlockRuleType.Exact:
                        if (prefabId == rule.Value) return true;
                        break;
                    case BlockRuleType.StartsWith:
                        if (prefabId.StartsWith(rule.Value)) return true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return false;
        }
    }
}
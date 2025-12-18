using System.Linq;
using Core.Scripts.Game.Infrastructure.ProjectNetworking.SpawnPointsLogic;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.ProjectNetworking.PlayersSpawner
{
    public sealed class PlayerSelfSpawner : SimulationBehaviour, IPlayerJoined
    {
        [SerializeField] private NetworkPrefabRef playerPrefab;

        public void PlayerJoined(PlayerRef player)
        {
            if (player != Runner.LocalPlayer || !playerPrefab.IsValid) return;
            
            var spawnPoint = FindObjectsByType<PlayerSpawnPoint>(FindObjectsSortMode.None);
            int spawnPointIndex = TryToGetSpawnPointIndex(spawnPoint.Length);
            
            SpawnPlayerAsync(player, spawnPoint[spawnPointIndex]).Forget();
        }

        private async UniTaskVoid SpawnPlayerAsync(PlayerRef player, PlayerSpawnPoint spawnPoint)
        {
            Vector3 spawnPosition = spawnPoint.transform.position;
            Quaternion spawnRotation = spawnPoint.RotateToFaceDirection;
            
            NetworkObject playerObj = await Runner.SpawnAsync(playerPrefab, spawnPosition, spawnRotation, player);
            Runner.SetPlayerObject(player, playerObj);
        }

        private int TryToGetSpawnPointIndex(int lenght)
        {
            int playersCount = Runner.ActivePlayers.Count();
            return playersCount < lenght ? playersCount : 0;
        }
    }
}
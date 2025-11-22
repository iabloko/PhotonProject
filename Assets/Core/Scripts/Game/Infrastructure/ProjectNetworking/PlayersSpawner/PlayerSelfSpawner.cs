using System.Linq;
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
            Debug.Log($"PlayerSpawner PlayerJoined - {player}");

            if (player != Runner.LocalPlayer) return;

            if (!playerPrefab.IsValid)
            {
                Debug.LogError(
                    "PlayerSelfSpawner: playerPrefabRef is INVALID. Rebuild Object Table and assign the ref.");
                return;
            }

            if (Runner.TryGetPlayerObject(player, out _))
            {
                Debug.LogError($"PlayerSelfSpawner: PlayerObject for {player} already exists. Skip spawn.");
                return;
            }

            var spawnPoint = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
            int spawnPointIndex = TryToGetSpawnPointIndex(spawnPoint.Length);
            
            SpawnPlayerAsync(player, spawnPoint[spawnPointIndex]).Forget();
        }

        private async UniTaskVoid SpawnPlayerAsync(PlayerRef player, SpawnPoint spawnPoint)
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
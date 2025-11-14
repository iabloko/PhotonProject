using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Sandbox.Project.Scripts.Infrastructure.PlayersSpawner
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
                Debug.LogError("PlayerSelfSpawner: playerPrefabRef is INVALID. Rebuild Object Table and assign the ref.");
                return;
            }

            if (Runner.TryGetPlayerObject(player, out _))
            {
                Debug.LogError($"PlayerSelfSpawner: PlayerObject for {player} already exists. Skip spawn.");
                return;
            }
            
            //TODO FIX
            Transform spawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;

             SpawnPlayerAsync(player, spawnPoint).Forget();
        }

        private async UniTaskVoid SpawnPlayerAsync(PlayerRef player, Transform spawnPoint)
        {
            NetworkObject playerObj = await Runner.SpawnAsync(playerPrefab, spawnPoint.position, spawnPoint.rotation, player);
            Runner.SetPlayerObject(player, playerObj);
        }
    }
}
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game
{
    public sealed class GameInitialization : MonoBehaviour
    {
        [Inject] private GameStartup _gameStartup;

        private void Start() => _gameStartup.Start();
    }
}
using Core.Scripts.Game.ScriptableObjects.Configs.Logger;
using Sirenix.Serialization;
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game.Installers
{
    [CreateAssetMenu(fileName = "GameLoggerInstaller", menuName = "Daniil/Installers/GameLoggerInstaller")]
    public sealed class GameLoggerInstaller : ScriptableObjectInstaller<GameLoggerInstaller>
    {
        [OdinSerialize] public GameLogger gameLogger;

        public override void InstallBindings()
        {
            Container.BindInstance(gameLogger).AsSingle();
        }
    }
}
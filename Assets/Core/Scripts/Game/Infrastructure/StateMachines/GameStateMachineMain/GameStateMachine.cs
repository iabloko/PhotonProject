using System;
using System.Collections.Generic;
using Core.Scripts.Game.Infrastructure.StateMachines.BaseData;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States;
using Core.Scripts.Game.ScriptableObjects.Configs.Logger;
using Zenject;

namespace Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain
{
    public sealed class GameStateMachine : GameStateMachineBase
    {
        private readonly GameLogger _logger;

        [Inject]
        public GameStateMachine(
            BootStrapState.Factory bootFactory,
            LoadLevelState.Factory loadLevelFactory,
            PhotonLobbyState.Factory photonFactory,
            GamePlayState.Factory gamePlayFactory,
            GameLogger logger)
        {
            _logger = logger;
            States = new Dictionary<Type, IExitState>
            {
                [typeof(BootStrapState)] = bootFactory.Create(this),
                [typeof(LoadLevelState)] = loadLevelFactory.Create(this),
                [typeof(PhotonLobbyState)] = photonFactory.Create(this),
                [typeof(GamePlayState)] = gamePlayFactory.Create(this),
            };
        }

        protected override void ShowLog<TState>(string message)
        {
            _logger.Log<TState>(LogLevel.Info, message);
        }
    }
}
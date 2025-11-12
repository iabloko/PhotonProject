using System;
using System.Collections.Generic;
using Core.Scripts.Game.Infrastructure.StateMachines.BaseData;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States;
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain
{
    public sealed class GameStateMachine : GameStateMachineBase
    {
        [Inject]
        public GameStateMachine(
            BootStrapState.Factory bootFactory,
            LoadLevelState.Factory loadLevelFactory,
            PhotonLobbyState.Factory photonFactory,
            GamePlayState.Factory gamePlayFactory)
        {
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
            try
            {
                Debug.Log($"{message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"{ex.Message}");
            }
        }
    }
}
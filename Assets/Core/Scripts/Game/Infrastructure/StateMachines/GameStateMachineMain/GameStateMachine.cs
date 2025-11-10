using System;
using System.Collections.Generic;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.StateMachines.BaseData;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain;
using Core.Scripts.Game.ScriptableObjects.Configs;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain
{
    public sealed class GameStateMachine : GameStateMachineBase
    {
        internal GameStateMachine(
            GameConfig config, IAssetProvider assetProvider, MainGameUIStateMachine uiStateMachine)
        {
            States = new Dictionary<Type, IExitState>
            {
                [typeof(BootStrapState)] = new BootStrapState(this),
                [typeof(LoadLevelState)] = new LoadLevelState(this, config),
                [typeof(GamePlayState)] = new GamePlayState(this, assetProvider, uiStateMachine),
            };
        }

        protected override void ShowLog<TState>(string message)
        {
            try
            {
                Debug.Log($"{"ENTER"}: {message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"{"ENTER"}: {ex.Message}");
            }
        }
    }
}
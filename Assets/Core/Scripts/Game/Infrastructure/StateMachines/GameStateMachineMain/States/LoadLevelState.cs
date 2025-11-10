using System;
using Core.Scripts.Game.Infrastructure.Loader;
using Core.Scripts.Game.Infrastructure.StateMachines.DataInterfaces;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States.Base;
using GameConfig = Core.Scripts.Game.ScriptableObjects.Configs.GameConfig;

namespace Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States
{
    public struct LoadLevelData : IData
    {
        public readonly string NextSceneName;
        public readonly Action OnSceneLoaded;

        public LoadLevelData(string nextSceneName, Action onSceneLoaded)
        {
            this.NextSceneName = nextSceneName;
            this.OnSceneLoaded = onSceneLoaded;
        }
    }

    internal sealed class LoadLevelState : PayloadStateBase<LoadLevelData>
    {
        public override string StateName => "LoadLevelState";

        private readonly SceneLoader _sceneLoader;

        public LoadLevelState(
            GameStateMachine gameGameStateMachineBase, GameConfig config) : base(gameGameStateMachineBase)
        {
            _sceneLoader = new SceneLoader();
        }

        public override void Enter(LoadLevelData payload)
        {
            _sceneLoader.MmSceneLoad(payload.NextSceneName, payload.OnSceneLoaded).Forget();
        }

        public override void Exit()
        {
        }
    }
}
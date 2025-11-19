using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States.Base;
using UnityEngine.Scripting;
using Zenject;

namespace Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States
{
    public sealed class GamePlayState : StateBase
    {
        private readonly IProjectSettings _settings;

        public GamePlayState(GameStateMachine gsm, IProjectSettings settings) : base(gsm)
        {
            _settings = settings;
        }

        public override string StateName => "GamePlayState";

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        [Preserve]
        public sealed class Factory : PlaceholderFactory<GameStateMachine, GamePlayState>
        {
            [Preserve]
            public Factory()
            {
            }
        }
    }
}
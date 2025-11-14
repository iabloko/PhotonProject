using Core.Scripts.Game.Infrastructure.StateMachines.DataInterfaces;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States.Base;
using UnityEngine.Scripting;
using Zenject;

namespace Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States
{
    public sealed class GamePlayState : StateBase
    {
        public GamePlayState(GameStateMachine gameGameStateMachineBase) : base(gameGameStateMachineBase)
        {
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
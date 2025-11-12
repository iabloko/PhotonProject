using Core.Scripts.Game.Infrastructure.StateMachines.DataInterfaces;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States.Base;
using UnityEngine.Scripting;
using Zenject;

namespace Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States
{
    public struct GamePlayStateData : IData
    {
    }

    public sealed class GamePlayState : PayloadStateBase<GamePlayStateData>
    {
        public GamePlayState(GameStateMachine gameGameStateMachineBase) : base(gameGameStateMachineBase)
        {
        }

        public override string StateName => "GamePlayState";

        public override void Enter(GamePlayStateData data)
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
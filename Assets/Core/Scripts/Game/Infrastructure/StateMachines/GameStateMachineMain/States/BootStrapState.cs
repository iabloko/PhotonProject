using Core.Scripts.Game.GameHelpers;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States.Base;

namespace Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States
{
    internal sealed class BootStrapState : StateBase
    {
        public override string StateName => "BootStrapState";

        public BootStrapState(GameStateMachine gameGameStateMachineBase) : base(gameGameStateMachineBase)
        {
        }
        
        public override void Enter()
        {
            GamePlayStateData data = new();

            GameStateMachine.Enter<LoadLevelState, LoadLevelData>(new LoadLevelData(
                GameConstant.GAME_PLAY,
                () => GameStateMachine.Enter<GamePlayState, GamePlayStateData>(data)));
        }

        public override void Exit()
        {
        }
    }
}
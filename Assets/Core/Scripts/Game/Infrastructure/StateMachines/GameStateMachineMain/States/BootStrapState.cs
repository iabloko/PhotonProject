using Core.Scripts.Game.GameHelpers;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States.Base;
using UnityEngine.Scripting;
using Zenject;

namespace Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States
{
    public sealed class BootStrapState : StateBase
    {
        public override string StateName => "BootStrapState";

        public BootStrapState(GameStateMachine gameGameStateMachineBase) : base(gameGameStateMachineBase)
        {
        }
        
        public override void Enter()
        {
            GameStateMachine.Enter<LoadLevelState, LoadLevelData>(
                new LoadLevelData(GameConstant.SCENE_TRANSITION,
                () => GameStateMachine.Enter<PhotonLobbyState>()));
        }

        public override void Exit()
        {
        }
        
        [Preserve]
        public class Factory : PlaceholderFactory<GameStateMachine, BootStrapState>
        {
            [Preserve]
            public Factory()
            {
            }
        }
    }
}
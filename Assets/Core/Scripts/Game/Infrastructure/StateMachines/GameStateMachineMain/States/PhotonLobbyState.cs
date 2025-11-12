using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States.Base;
using UnityEngine.Scripting;
using Zenject;

namespace Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States
{
    public sealed class PhotonLobbyState : StateBase
    {
        public PhotonLobbyState(GameStateMachine gameStateMachineBaseBase) : base(gameStateMachineBaseBase)
        {
        }

        public override string StateName => "PhotonLobbyState";
        
        public override void Enter()
        {
        }

        public override void Exit()
        {
        }
        
        [Preserve]
        public class Factory : PlaceholderFactory<GameStateMachine, PhotonLobbyState>
        {
            [Preserve]
            public Factory()
            {
            }
        }
    }
}
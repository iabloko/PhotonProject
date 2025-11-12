using System.Threading;
using Core.Scripts.Game.Infrastructure.StateMachines.BaseData;

namespace Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.States.Base
{
    public abstract class GameMenuUIStateBase<TPayload> : AsyncPayloadState<TPayload>
        where TPayload : IPayload
    {
        public MainGameUIStateMachine StateMachine { get; set; }
        protected CancellationTokenSource TokenSource;
    }    
    
    public abstract class GameMenuUIStateBase : AsyncState
    {
        public MainGameUIStateMachine StateMachine { get; set; }
        protected CancellationTokenSource TokenSource;
    }
}
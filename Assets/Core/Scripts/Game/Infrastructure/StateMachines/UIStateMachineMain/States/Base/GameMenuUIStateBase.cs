using System.Threading;
using Core.Scripts.Game.Infrastructure.StateMachines.BaseData;

namespace Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.States.Base
{
    public abstract class GameMenuUIStateBase<TPayload> : AsyncAsyncPayloadState<TPayload>
        where TPayload : IPayload
    {
        public MainGameUIStateMachine StateMachine { get; set; }

        protected CancellationTokenSource TokenSource;
    }
}
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain;
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game.Infrastructure.Bootstrapper
{
    public sealed class GamePlayBootstrapper : MonoBehaviour
    {
        private MainGameUIStateMachine _uiStateMachine;

        [Inject]
        public void Construct(MainGameUIStateMachine uiStateMachine)
        {
            _uiStateMachine = uiStateMachine;
        }

        private void Start()
        {
            _uiStateMachine.SetUpStateMachine().Forget();
        }
    }
}
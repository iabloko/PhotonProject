using Core.Scripts.Game.Infrastructure.Services.Input;
using Fusion;
using Fusion.Addons.SimpleKCC;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game.PlayerLogic.InputLogic
{
    public struct InputModelData : INetworkInput
    {
        public const int JUMP_BUTTON = 0;
        public const int FIRST_PERSON_BUTTON = 2;
        public const int COPY_BUTTON = 3;
        public const int SCALE_BUTTON = 4;
        public const int SHIFT_BUTTON = 5;
        public const int ATTACK_BUTTON = 6;
        
        public float DragZoomDelta;
        
        public Vector2 MoveDirection;
        public Vector2 LookRotationDelta;
        public NetworkButtons Actions;
    }
    
    [DefaultExecutionOrder(-10)]
    public sealed class PlayerInput : NetworkBehaviour, IBeforeUpdate, IBeforeTick
    {
        private const float MAX_STEP = 5f;
        public InputModelData CurrentInput => _currentInput;
        public InputModelData PreviousInput => _previousInput;

        public float ScrollWheel => _keyHandler.Scroll;
        public float ScrollWheelRaw => _keyHandler.ScrollRaw;

        private IKeyHandler _keyHandler;

        [Title("Mouse", "delta multiplier", TitleAlignments.Right), SerializeField]
        private Vector2 lookSensitivity = Vector2.one;

        [Networked] private InputModelData _currentInput { get; set; }

        private InputModelData _previousInput;
        private InputModelData _accumulatedInput;

        private bool _resetAccumulatedInput;
        private Vector2Accumulator _lookRotationAccumulator;
        
        [Inject]
        public void Constructor(IKeyHandler keyHandler) => _keyHandler = keyHandler;

        public override void Spawned()
        {
            _currentInput = default;
            _previousInput = default;
            _accumulatedInput = default;
            _resetAccumulatedInput = default;

            if (Object.HasInputAuthority)
            {
                NetworkEvents networkEvents = Runner.GetComponent<NetworkEvents>();
                networkEvents.OnInput.AddListener(OnInput);
                _lookRotationAccumulator = new Vector2Accumulator(Runner.DeltaTime, true);

                ReplicateToAll(false);
                ReplicateTo(Runner.LocalPlayer, true);
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (runner == null) return;

            NetworkEvents networkEvents = runner.GetComponent<NetworkEvents>();
            if (networkEvents != null)
            {
                networkEvents.OnInput.RemoveListener(OnInput);
            }
        }
        
        void IBeforeUpdate.BeforeUpdate()
        {
            if (!Object.HasInputAuthority) return;
            
            if (_resetAccumulatedInput)
            {
                _resetAccumulatedInput = false;
                _accumulatedInput = default;
            }

            Vector2 mouseDelta = _keyHandler.RotationData * lookSensitivity;

            Vector2 safeDelta = new(
                Mathf.Clamp(-mouseDelta.y, -MAX_STEP, MAX_STEP),
                Mathf.Clamp(mouseDelta.x, -MAX_STEP, MAX_STEP)
            );
            
            _accumulatedInput.MoveDirection = _keyHandler.MovementData;

            _lookRotationAccumulator.Accumulate(safeDelta);

            _accumulatedInput.MoveDirection = _keyHandler.MovementData;
            _accumulatedInput.DragZoomDelta = _keyHandler.ScrollRaw;
            
            _accumulatedInput.Actions.Set(InputModelData.JUMP_BUTTON, _keyHandler.IsJumping);
            _accumulatedInput.Actions.Set(InputModelData.SHIFT_BUTTON, _keyHandler.IsShifting);
            _accumulatedInput.Actions.Set(InputModelData.ATTACK_BUTTON, _keyHandler.IsAttack);
            _accumulatedInput.Actions.Set(InputModelData.FIRST_PERSON_BUTTON, _keyHandler.IsFirstPersonButtonPressed);
        }

        void IBeforeTick.BeforeTick()
        {
            if (Object == null) return;

            _previousInput = _currentInput;

            InputModelData currentInput = _currentInput;
            currentInput.LookRotationDelta = default;
            _currentInput = currentInput;

            if (!Object.HasInputAuthority) return;

            if (GetInput(out InputModelData input))
            {
                _currentInput = input;
            }
        }

        private void OnInput(NetworkRunner runner, NetworkInput networkInput)
        {
            _accumulatedInput.LookRotationDelta = _lookRotationAccumulator.ConsumeTickAligned(runner);

            networkInput.Set(_accumulatedInput);

            _resetAccumulatedInput = true;
        }
    }
}
using Core.Scripts.Game.Infrastructure.Services.KeyHandlerService;
using Fusion;
using Fusion.Addons.SimpleKCC;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game.Player.NetworkInput
{
    public struct InputModelData : INetworkInput
    {
        public const int JUMP_BUTTON = 0;
        public const int DRAG_BUTTON = 2;
        public const int COPY_BUTTON = 3;
        public const int SCALE_BUTTON = 4;
        public const int EMPTY_BACKPACK_BUTTON = 5;
        
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

        internal float ScrollWheel => KeyHandler.Scroll;
        internal float ScrollWheelRaw => KeyHandler.ScrollRaw;
        internal bool IsProfileButtonPressed => KeyHandler.IsProfileButtonPressed;

        internal IKeyHandler KeyHandler;

        [Title("Mouse", "delta multiplier", TitleAlignments.Right), SerializeField]
        private Vector2 lookSensitivity = Vector2.one;

        [Networked] private InputModelData _currentInput { get; set; }

        private InputModelData _previousInput;
        private InputModelData _accumulatedInput;

        private bool _resetAccumulatedInput;
        private Vector2Accumulator _lookRotationAccumulator;
        private bool _copyLatched;
        private bool _emptyBackPackLatched;
        
        [Inject]
        public void Constructor(IKeyHandler keyHandler)
        {
            KeyHandler = keyHandler;
        }

        public override void Spawned()
        {
            _currentInput = default;
            _previousInput = default;
            _accumulatedInput = default;
            _resetAccumulatedInput = default;

            if (Object.HasInputAuthority)
            {
                // lookSensitivity = PlayerInfo.LookSensitivity;
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

            Debug.Log($"PlayerInput Despawned");
        }

        public void ChangeInputSensitivity(SensitivityType type, float value)
        {
            if (type == SensitivityType.Oy)
            {
                lookSensitivity.y = value;
            }
            else
            {
                lookSensitivity.x = value;
            }

            // PlayerInfo.SaveSensitivitySettings(lookSensitivity);
        }

        void IBeforeUpdate.BeforeUpdate()
        {
            // if (HasStateAuthority == false || ProjectSettings.IsCursorLocked == false) return;
            if (!Object.HasInputAuthority) return;
            
            if (KeyHandler.IsCopyButtonPressed) _copyLatched = true;
            if (KeyHandler.IsAlpha1Pressed) _emptyBackPackLatched = true;

            if (_resetAccumulatedInput)
            {
                _resetAccumulatedInput = false;
                _accumulatedInput = default;
            }

            Vector2 mouseDelta = KeyHandler.RotationData * lookSensitivity;

            Vector2 safeDelta = new(
                Mathf.Clamp(-mouseDelta.y, -MAX_STEP, MAX_STEP),
                Mathf.Clamp(mouseDelta.x, -MAX_STEP, MAX_STEP)
            );
            
            _accumulatedInput.MoveDirection = KeyHandler.MovementData;
            _accumulatedInput.Actions.Set(InputModelData.JUMP_BUTTON, KeyHandler.IsJumping);

            _lookRotationAccumulator.Accumulate(safeDelta);

            _accumulatedInput.MoveDirection = KeyHandler.MovementData;
            _accumulatedInput.DragZoomDelta = KeyHandler.ScrollRaw;
            
            _accumulatedInput.Actions.Set(InputModelData.JUMP_BUTTON, KeyHandler.IsJumping);
            _accumulatedInput.Actions.Set(InputModelData.DRAG_BUTTON, KeyHandler.IsPrimaryHeld);
            _accumulatedInput.Actions.Set(InputModelData.SCALE_BUTTON, KeyHandler.IsScaleHeld);
            _accumulatedInput.Actions.Set(InputModelData.COPY_BUTTON, _copyLatched);
            _accumulatedInput.Actions.Set(InputModelData.EMPTY_BACKPACK_BUTTON, _emptyBackPackLatched);
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
            // else
            // {
            // Debug.LogWarning("PlayerInput BeforeTick GetInput = false (ProvideInput off? input не вызван?)");
            // }
        }

        private void OnInput(NetworkRunner runner, Fusion.NetworkInput networkInput)
        {
            _accumulatedInput.LookRotationDelta = _lookRotationAccumulator.ConsumeTickAligned(runner);

            networkInput.Set(_accumulatedInput);

            _resetAccumulatedInput = true;
            _copyLatched = false;
            _emptyBackPackLatched = false;
        }
    }
}
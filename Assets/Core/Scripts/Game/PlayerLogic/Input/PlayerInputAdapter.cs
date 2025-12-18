using Core.Scripts.Game.CharacterLogic;
using UnityEngine;

namespace Core.Scripts.Game.PlayerLogic.Input
{
    public sealed class PlayerInputAdapter : ICharacterInput
    {
        private readonly PlayerInput _input;

        public PlayerInputAdapter(PlayerInput input) => _input = input;

        public Vector2 Move => _input.CurrentInput.MoveDirection;
        public Vector2 LookDelta => _input.CurrentInput.LookRotationDelta;

        public float Scroll => _input.ScrollWheel;
        
        public bool SprintHeld => UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift);

        public bool JumpPressed =>
            _input.CurrentInput.Actions.WasPressed(_input.PreviousInput.Actions, InputModelData.JUMP_BUTTON);

        public bool AttackPressed =>
            _input.CurrentInput.Actions.WasPressed(_input.PreviousInput.Actions, InputModelData.ATTACK_BUTTON);

        public bool ToggleFpsPressed => UnityEngine.Input.GetKeyDown(KeyCode.V);
    }
}
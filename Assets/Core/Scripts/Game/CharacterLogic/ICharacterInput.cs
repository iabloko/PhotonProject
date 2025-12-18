using UnityEngine;

namespace Core.Scripts.Game.CharacterLogic
{
    public interface ICharacterInput
    {
        public Vector2 Move { get; }
        public Vector2 LookDelta { get; }
        public float Scroll { get; }
        public bool SprintHeld { get; }

        public bool JumpPressed { get; }
        public bool AttackPressed { get; }

        public bool ToggleFpsPressed { get; }
    }
}
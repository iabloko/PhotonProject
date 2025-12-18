using UnityEngine;

namespace Core.Scripts.Game.CharacterLogic
{
    public interface ICharacterInput
    {
        Vector2 Move { get; }
        Vector2 LookDelta { get; }
        float Scroll { get; }
        bool SprintHeld { get; }

        bool JumpPressed { get; }
        bool AttackPressed { get; }

        bool ToggleFpsPressed { get; }
    }
}
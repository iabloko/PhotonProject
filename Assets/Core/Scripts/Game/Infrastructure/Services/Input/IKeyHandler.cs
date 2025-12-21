using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.Input
{
    public interface IKeyHandler
    {
        public bool IsJumping { get; }
        public bool IsShifting { get; }
        public bool IsAttack { get; }
        public bool IsFirstPersonButtonPressed { get; }
        
        public float Scroll { get; }
        public float ScrollRaw { get; }

        public Vector2 MovementData { get; }
        public Vector2 RotationData { get; }
    }
}
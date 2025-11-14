using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.KeyHandlerService
{
    public interface IKeyHandler
    {
        public bool IsJumping { get; set; }
        public bool IsShifting { get; set; }
        public bool IsPrimaryHeld { get; set; }
        public bool IsScaleHeld { get; set; }
        public bool IsCopyButtonPressed { get; set; }
        public bool IsProfileButtonPressed { get; set; }

        public bool IsAlpha1Pressed { get; set; }

        public float Scroll { get; }
        public float ScrollRaw { get; }

        public Vector2 MovementData { get; }
        public Vector2 RotationData { get; }

        public void ResetData();
    }
}
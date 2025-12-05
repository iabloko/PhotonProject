using UnityEngine;

namespace Core.Scripts.Game.Player.NetworkInput
{
    public interface IKeyHandler
    {
        public bool IsJumping { get; set; }
        public bool IsShifting { get; set; }
        
        public float Scroll { get; }
        public float ScrollRaw { get; }

        public Vector2 MovementData { get; }
        public Vector2 RotationData { get; }

        public void ResetData();
    }
}
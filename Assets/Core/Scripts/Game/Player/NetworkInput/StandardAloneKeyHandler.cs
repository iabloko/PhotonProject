using UnityEngine;

namespace Core.Scripts.Game.Player.NetworkInput
{
    public sealed class StandardAloneKeyHandler : IKeyHandler
    {
        private const string MOUSE_Y = "Mouse Y";
        private const string MOUSE_X = "Mouse X";

        private const string HORIZONTAL = "Horizontal";
        private const string VERTICAL = "Vertical";
        private const string MOUSE_SCROLL_WHEEL = "Mouse ScrollWheel";
        private const float MIN_SCROLL = 2;
        private const float MAX_SCROLL = 15;

        private bool _isJumping;
        private bool _isShifting;

        private float _scroll;

        public bool IsJumping
        {
            get => Input.GetKey(KeyCode.Space);
            set => _isJumping = value;
        }
        
        public bool IsShifting
        {
            get => Input.GetKey(KeyCode.LeftShift);
            set => _isShifting = value;
        }

        public float Scroll
        {
            get
            {
                _scroll -= Input.GetAxis(MOUSE_SCROLL_WHEEL);
                _scroll = Mathf.Clamp(_scroll, MIN_SCROLL, MAX_SCROLL);
                return _scroll;
            }
        }

        public float ScrollRaw => Input.GetAxis(MOUSE_SCROLL_WHEEL);

        public Vector2 MovementData => new(Input.GetAxis(HORIZONTAL), Input.GetAxis(VERTICAL));
        public Vector2 RotationData
        {
            get
            {
                float x = Input.GetAxis(MOUSE_X);
                float y = Input.GetAxis(MOUSE_Y);

                if (x == 0)
                {
                    x = Input.GetKey(KeyCode.LeftArrow) ? -1f : (Input.GetKey(KeyCode.RightArrow) ? 1f : 0f);
                }

                if (y == 0)
                {
                    y = Input.GetKey(KeyCode.DownArrow) ? -1f : (Input.GetKey(KeyCode.UpArrow) ? 1f : 0f);
                }

                return new Vector2(x, y);
            }
        }

        public void ResetData() => IsJumping = false;
    }
}
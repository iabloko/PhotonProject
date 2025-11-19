using Core.Scripts.Game.Player.Animations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.Player.Locomotion
{
    public abstract class PlayerBaseAnimation : PlayerBaseData
    {
        [Title("Animation", "", TitleAlignments.Right), SerializeField]
        protected Animator animator;
        private AnimationController _animationController;

        private float _horizontal;
        private float _vertical;

        private const float EPSILON = .01f;
        private const float EPSILON_MAX = 0.99f;

        public override void Spawned()
        {
            base.Spawned();
            _animationController = new AnimationController(animator);
        }
        
        protected virtual void LateUpdate()
        {
            if (ProjectSettings.IsGamePaused)
            {
                _vertical = _horizontal = 0;
                RestartPlayerAnimations();
                return;
            }
            
            _vertical = CalculateVertical();
            _horizontal = CalculateHorizontal();

            _animationController.PlayJump(kcc.IsGrounded);
            _animationController.PlayMovement(IsPlayerMovingOrFalling);
            _animationController.PlayVertical(_vertical);
            _animationController.PlayHorizontal(_horizontal);
        }

        protected void JumpAnimation() => _animationController.PlayJump(true);
        
        private void RestartPlayerAnimations()
        {
            _animationController.PlayJump(kcc.IsGrounded);
            _animationController.PlayMovement(false);

            _animationController.PlayVertical(_vertical);
            _animationController.PlayHorizontal(_horizontal);
        }

        private float CalculateVertical()
        {
            if (roomData.settings.autoRun) return 1;

            float vertical = Mathf.Lerp(_vertical, input.CurrentInput.MoveDirection.y, 0.1f);

            if (Mathf.Abs(vertical) < EPSILON) return 0f;

            return Mathf.Abs(vertical) > EPSILON_MAX ? Mathf.Sign(vertical) : vertical;
        }

        private float CalculateHorizontal()
        {
            float horizontal = Mathf.Lerp(_horizontal, input.CurrentInput.MoveDirection.x, 0.1f);

            if (Mathf.Abs(horizontal) < EPSILON) return 0f;

            return Mathf.Abs(horizontal) > EPSILON_MAX ? Mathf.Sign(horizontal) : horizontal;
        }
    }
}
using UnityEngine;

namespace Core.Scripts.Game.Player.Animations
{
    public sealed class AnimationController
    {
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int IsAirborne = Animator.StringToHash("IsAirborne");
        private static readonly int IsMovement = Animator.StringToHash("IsMovement");

        private readonly Animator _animator;

        public AnimationController(Animator animator)
        {
            _animator = animator;
        }
        
        public void PlayJump(bool status)
        {
            _animator.SetBool(IsAirborne, !status);
        }

        public void PlayMovement(bool status)
        {
            _animator.SetBool(IsMovement, status);
        }

        public void PlayVertical(float value)
        {
            _animator.SetFloat(Vertical, value);
        }

        public void PlayHorizontal(float value)
        {
            _animator.SetFloat(Horizontal, value);
        }

        public void OverrideAnimatorController(AnimatorOverrideController controller)
        {
            _animator.runtimeAnimatorController = controller;
        }
    }
}
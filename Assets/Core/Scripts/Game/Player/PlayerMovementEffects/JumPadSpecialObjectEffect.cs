using UnityEngine;

namespace Core.Scripts.Game.Player.PlayerMovementEffects
{
    public sealed class JumPadSpecialObjectEffect : SpecialObjectEffect
    {
        private readonly Animator _animator;
        private static readonly int IsJumped = Animator.StringToHash("IsJumped");

        public JumPadSpecialObjectEffect(Animator animator, float duration) : base(duration)
        {
            _animator = animator;
        }

        public override void Start()
        {
            _animator.SetBool(IsJumped, true);
        }        
        
        public override void Stop()
        {
        }
    }
}
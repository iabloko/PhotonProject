using UnityEngine;

namespace Core.Scripts.Game.Player.MovementEffects
{
    public sealed class BubbleGumSpecialObjectEffect : SpecialObjectEffect
    {
        private static readonly int IsBubble = Animator.StringToHash("IsBubbleGum");
        private readonly Animator _animator;

        public BubbleGumSpecialObjectEffect(Animator animator, float d) : base(d)
        {
            _animator = animator;
        }

        public override void Start()
        {
            base.Start();
            _animator.SetBool(IsBubble, true);
        }

        public override void Stop()
        {
            base.Stop();
            _animator.SetBool(IsBubble, false);
        }
    }
}
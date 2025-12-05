using UnityEngine;

namespace Core.Scripts.Game.Player.MovementEffects
{
    public sealed class BananaSpecialObjectEffect : SpecialObjectEffect
    {
        private static readonly int IsBanana = Animator.StringToHash("IsBanana");
        private readonly Animator _animator;
        private bool _forceSkip;

        public BananaSpecialObjectEffect(Animator animator, float d) : base(d)
        {
            _animator = animator;
        }

        public override void Start()
        {
            base.Start();
            _animator.SetBool(IsBanana, true);
        }

        public override void Stop()
        {
            base.Stop();
            _animator.SetBool(IsBanana, false);
        }
    }
}
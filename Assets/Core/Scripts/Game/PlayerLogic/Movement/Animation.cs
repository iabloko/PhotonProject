using Core.Scripts.Game.Infrastructure.ModelData;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using Core.Scripts.Game.PlayerLogic.ContextLogic;
using UnityEngine;

namespace Core.Scripts.Game.PlayerLogic.Movement
{
    public sealed class Animation
    {
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int IsAirborne = Animator.StringToHash("IsAirborne");
        private static readonly int IsMovement = Animator.StringToHash("IsMovement");
        private static readonly int IsCombat = Animator.StringToHash("InCombat");
        private static readonly int AttackSequence = Animator.StringToHash("AttackSequence");

        private readonly Animator _animator;
        private readonly RoomSettings _roomData;
        private readonly PlayerContext _context;
        
        private float _horizontal;
        private float _vertical;
        private readonly IProjectSettings _projectSettings;

        private const float EPSILON = .01f;
        private const float EPSILON_MAX = 0.99f;

        public Animation(PlayerContext c, Animator a, IProjectSettings p, RoomSettings r)
        {
            _projectSettings = p;
            _context = c;
            _roomData = r;
            _animator = a;
        }

        public void PlayJump(bool status)
        {
            _animator.SetBool(IsAirborne, !status);
        }

        public void OverrideAnimatorController(AnimatorOverrideController controller)
        {
            _animator.runtimeAnimatorController = controller;
        }

        public void LateUpdate()
        {
            if (_projectSettings.IsGamePaused)
            {
                _vertical = _horizontal = 0;
                RestartPlayerAnimations();
                return;
            }
            
            _vertical = CalculateVertical();
            _horizontal = CalculateHorizontal();

            PlayJump(_context.Kcc.IsGrounded);
            PlayMovement(_context.IsMovingOrFalling);
            PlayVertical(_vertical);
            PlayHorizontal(_horizontal);
        }
        
        public void SetCombatStatus(bool isInCombat)
        {
            _animator.SetBool(IsCombat, isInCombat);
        }
        
        public void SetAttackAnimation(int attackSequence)
        {
            _animator.SetInteger(AttackSequence, attackSequence);
        }

        private void PlayMovement(bool status)
        {
            _animator.SetBool(IsMovement, status);
        }

        private void PlayVertical(float value)
        {
            _animator.SetFloat(Vertical, value);
        }

        private void PlayHorizontal(float value)
        {
            _animator.SetFloat(Horizontal, value);
        }

        private float CalculateVertical()
        {
            if (_roomData.settings.autoRun) return 1;

            float vertical = Mathf.Lerp(_vertical, _context.Input.CurrentInput.MoveDirection.y, 0.1f);

            if (Mathf.Abs(vertical) < EPSILON) return 0f;

            return Mathf.Abs(vertical) > EPSILON_MAX ? Mathf.Sign(vertical) : vertical;
        }

        private float CalculateHorizontal()
        {
            float horizontal = Mathf.Lerp(_horizontal, _context.Input.CurrentInput.MoveDirection.x, 0.1f);

            if (Mathf.Abs(horizontal) < EPSILON) return 0f;

            return Mathf.Abs(horizontal) > EPSILON_MAX ? Mathf.Sign(horizontal) : horizontal;
        }

        private void RestartPlayerAnimations()
        {
            PlayJump(_context.Kcc.IsGrounded);
            PlayMovement(false);

            PlayVertical(_vertical);
            PlayHorizontal(_horizontal);
        }
    }
}
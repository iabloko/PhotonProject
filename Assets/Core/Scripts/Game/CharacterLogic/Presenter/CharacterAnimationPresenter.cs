using Core.Scripts.Game.Infrastructure.ModelData;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using UnityEngine;

namespace Core.Scripts.Game.CharacterLogic.Presenter
{
    public sealed class CharacterAnimationPresenter
    {
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int IsAirborne = Animator.StringToHash("IsAirborne");
        private static readonly int IsMovement = Animator.StringToHash("IsMovement");
        private static readonly int IsCombat = Animator.StringToHash("InCombat");
        private static readonly int AttackSequence = Animator.StringToHash("AttackSequence");

        private const float EPSILON = 0.01f;
        private const float EPSILON_MAX = 0.9f;

        private readonly ICharacterMotor _motor;
        private readonly Animator _animator;
        private readonly IProjectSettings _projectSettings;
        private readonly GameplaySettings _gameplayData;

        private float _vertical;
        private float _horizontal;

        public CharacterAnimationPresenter(ICharacterMotor motor, Animator animator, IProjectSettings projectSettings, GameplaySettings gameplayData)
        {
            _motor = motor;
            _animator = animator;
            _projectSettings = projectSettings;
            _gameplayData = gameplayData;
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

            PlayJump(_motor.IsGrounded);
            PlayMovement(_motor.RealVelocity.sqrMagnitude > 0.02f || !_motor.IsGrounded);
            PlayVertical(_vertical);
            PlayHorizontal(_horizontal);
        }

        public void OverrideAnimatorController(AnimatorOverrideController controller)
        {
            if (controller == null) return;
            _animator.runtimeAnimatorController = controller;
        }

        public void SetCombatStatus(bool isInCombat) => _animator.SetBool(IsCombat, isInCombat);
        public void SetAttackAnimation(int attackSequence) => _animator.SetInteger(AttackSequence, attackSequence);

        public void PlayJump(bool grounded) => _animator.SetBool(IsAirborne, !grounded);

        private float CalculateVertical()
        {
            Vector3 localVel = _motor.Transform.InverseTransformDirection(_motor.RealVelocity);
            float maxSpeed = Mathf.Max(_gameplayData.settings.runningSpeed, _gameplayData.settings.walkingSpeed, 0.01f);
            float to = localVel.z / maxSpeed;
            
            float target = Mathf.Clamp(to, -1f, 1f);
            float v = Mathf.Lerp(_vertical, target, 0.1f);

            if (Mathf.Abs(v) < EPSILON) return 0f;
            return Mathf.Abs(v) > EPSILON_MAX ? Mathf.Sign(v) : v;
        }

        private float CalculateHorizontal()
        {
            Vector3 localVel = _motor.Transform.InverseTransformDirection(_motor.RealVelocity);
            float maxSpeed = Mathf.Max(_gameplayData.settings.runningSpeed, _gameplayData.settings.walkingSpeed, 0.01f);

            float target = Mathf.Clamp(localVel.x / maxSpeed, -1f, 1f);
            float h = Mathf.Lerp(_horizontal, target, 0.1f);

            if (Mathf.Abs(h) < EPSILON) return 0f;
            return Mathf.Abs(h) > EPSILON_MAX ? Mathf.Sign(h) : h;
        }

        private void PlayVertical(float v) => _animator.SetFloat(Vertical, v);
        private void PlayHorizontal(float h) => _animator.SetFloat(Horizontal, h);
        private void PlayMovement(bool status) => _animator.SetBool(IsMovement, status);

        private void RestartPlayerAnimations()
        {
            PlayJump(_motor.IsGrounded);
            PlayMovement(false);
            PlayVertical(_vertical);
            PlayHorizontal(_horizontal);
        }
    }
}

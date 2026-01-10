using System;
using Core.Scripts.Game.Combat.Data;
using UnityEngine;

namespace Core.Scripts.Game.Combat.PercentageArmorCalculator
{
    /// <summary>
    /// Презентер эффектов урона: хитмаркеры, числа урона, вспышки при попадании.
    /// </summary>
    public sealed class DamageEffectsPresenter
    {
        private readonly ParticleSystem _hitParticles;
        private readonly Action<DamageEvent> _onShowDamageNumber;

        public DamageEffectsPresenter(ParticleSystem hitParticles, Action<DamageEvent> onShowDamageNumber)
        {
            _hitParticles = hitParticles;
            _onShowDamageNumber = onShowDamageNumber;
        }

        public void PlayHitEffect(DamageEvent damageEvent)
        {
            PlayHitParticles(damageEvent.HitPoint, damageEvent.HitDirection);
            _onShowDamageNumber?.Invoke(damageEvent);
        }

        private void PlayHitParticles(Vector3 position, Vector3 direction)
        {
            if (_hitParticles == null) return;

            _hitParticles.transform.position = position;
            _hitParticles.transform.rotation = Quaternion.LookRotation(direction);
            _hitParticles.Play();
        }
    }

    /// <summary>
    /// Презентер UI здоровья: полоска HP, визуал получения урона.
    /// </summary>
    public sealed class HealthPresenter
    {
        private readonly UnityEngine.UI.Image _healthBar;
        private readonly CanvasGroup _damageFlash;
        private readonly float _flashDuration;

        private float _flashTimer;
        private float _displayedHealth;
        private float _targetHealth;
        private readonly float _lerpSpeed;

        public HealthPresenter(
            UnityEngine.UI.Image healthBar = null,
            CanvasGroup damageFlash = null,
            float flashDuration = 0.2f,
            float lerpSpeed = 10f)
        {
            _healthBar = healthBar;
            _damageFlash = damageFlash;
            _flashDuration = flashDuration;
            _lerpSpeed = lerpSpeed;

            _displayedHealth = 1f;
            _targetHealth = 1f;
        }

        public void SetHealth(float normalizedHealth)
        {
            _targetHealth = Mathf.Clamp01(normalizedHealth);
        }

        public void OnDamageReceived()
        {
            _flashTimer = _flashDuration;

            if (_damageFlash != null)
                _damageFlash.alpha = 1f;
        }

        public void Update(float deltaTime)
        {
            UpdateHealthBar(deltaTime);
            UpdateDamageFlash(deltaTime);
        }

        private void UpdateHealthBar(float deltaTime)
        {
            if (_healthBar == null) return;

            _displayedHealth = Mathf.Lerp(_displayedHealth, _targetHealth, _lerpSpeed * deltaTime);
            _healthBar.fillAmount = _displayedHealth;
        }

        private void UpdateDamageFlash(float deltaTime)
        {
            if (_damageFlash == null) return;
            if (_flashTimer <= 0f) return;

            _flashTimer -= deltaTime;
            _damageFlash.alpha = Mathf.Clamp01(_flashTimer / _flashDuration);
        }
    }

    /// <summary>
    /// Презентер смерти: анимация, ragdoll, respawn UI.
    /// </summary>
    public sealed class DeathPresenter
    {
        private readonly Animator _animator;
        private readonly GameObject _ragdollRoot;
        private readonly Action _onDeathAnimationComplete;

        private static readonly int DeathTrigger = Animator.StringToHash("Death");
        private static readonly int IsDeadBool = Animator.StringToHash("IsDead");

        public DeathPresenter(
            Animator animator,
            GameObject ragdollRoot = null,
            Action onDeathAnimationComplete = null)
        {
            _animator = animator;
            _ragdollRoot = ragdollRoot;
            _onDeathAnimationComplete = onDeathAnimationComplete;
        }

        public void PlayDeath(bool useRagdoll = false)
        {
            if (useRagdoll && _ragdollRoot != null)
            {
                EnableRagdoll();
            }
            else if (_animator != null)
            {
                _animator.SetBool(IsDeadBool, true);
                _animator.SetTrigger(DeathTrigger);
            }
        }

        public void Reset()
        {
            if (_animator != null)
                _animator.SetBool(IsDeadBool, false);

            if (_ragdollRoot != null)
                DisableRagdoll();
        }

        private void EnableRagdoll()
        {
            if (_animator != null)
                _animator.enabled = false;

            Rigidbody[] bodies = _ragdollRoot.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in bodies)
            {
                rb.isKinematic = false;
            }
        }

        private void DisableRagdoll()
        {
            Rigidbody[] bodies = _ragdollRoot.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in bodies)
            {
                rb.isKinematic = true;
            }

            if (_animator != null)
                _animator.enabled = true;
        }
    }
}

using UnityEngine;

namespace Core.Scripts.Game.Player.PlayerMovementEffects
{
    public abstract class SpecialObjectEffect
    {
        public bool IsActiveNow { get; private set; }

        private readonly float _moverDuration;
        private float _curTimer;
        
        protected SpecialObjectEffect(float duration)
        {
            _moverDuration = duration;
        }

        public virtual void Start()
        {
            IsActiveNow = true;
            _curTimer = 0;
        }

        public virtual void Stop()
        {
            IsActiveNow = false;
            _curTimer = 0;
        }

        public virtual void UpdateEffectTimer()
        {
            if (!IsActiveNow) return;

            _curTimer += Time.deltaTime;

            if (_curTimer < _moverDuration) return;

            Stop();
        }
    }
}
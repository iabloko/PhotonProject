using System;
using UnityEngine;

namespace Core.Scripts.Game.CharacterLogic.CharacterCombat
{
    public sealed class CombatStateMachine
    {
        private readonly Func<int> _getSequence;
        private readonly Action<int> _setSequence;

        private readonly Func<int> _getLastAttackTick;
        private readonly Action<int> _setLastAttackTick;

        private readonly Func<int> _getTick;
        private readonly Func<float> _getDeltaTime;

        private readonly int _maxCombo;
        private readonly float _resetSeconds;

        public CombatStateMachine(
            Func<int> getSequence, 
            Action<int> setSequence,
            Func<int> getLastAttackTick, 
            Action<int> setLastAttackTick,
            Func<int> getTick, 
            Func<float> getDeltaTime,
            int maxCombo, float resetSeconds)
        {
            _getSequence = getSequence;
            _setSequence = setSequence;
            _getLastAttackTick = getLastAttackTick;
            _setLastAttackTick = setLastAttackTick;
            _getTick = getTick;
            _getDeltaTime = getDeltaTime;
            _maxCombo = Mathf.Max(1, maxCombo);
            _resetSeconds = Mathf.Max(0.01f, resetSeconds);
        }

        public void OnAttack()
        {
            _setLastAttackTick(_getTick());

            int next = _getSequence() + 1;
            if (next > _maxCombo) next = 1;

            _setSequence(next);
        }

        public void FixedUpdate()
        {
            if (_getSequence() == 0) return;

            int ticksForReset = Mathf.CeilToInt(_resetSeconds / _getDeltaTime());
            if ((_getTick() - _getLastAttackTick()) >= ticksForReset)
                _setSequence(0);
        }
    }
}
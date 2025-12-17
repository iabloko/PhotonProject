using System;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using Core.Scripts.Game.PlayerLogic.ContextLogic;
using Core.Scripts.Game.PlayerLogic.NetworkInput;
using UnityEngine;

namespace Core.Scripts.Game.PlayerLogic.Combat
{
    public sealed class PlayerCombat : IDisposable
    {
        private event Action OnAttack;
        private readonly PlayerContext _context;
        private readonly IProjectSettings _projectSettings;

        public PlayerCombat(PlayerContext c, IProjectSettings p, Action attackSequence)
        {
            _context = c;
            _projectSettings = p;
            OnAttack = attackSequence;
        }

        public void FixedUpdateNetwork()
        {
            bool pressed = _context.Input.CurrentInput.Actions.WasPressed(
                _context.Input.PreviousInput.Actions, InputModelData.ATTACK_BUTTON);

            if (pressed)
            {
                Debug.Log("[PlayerCombat] FixedUpdateNetwork OnAttack");
                OnAttack?.Invoke();
            }
        }

        public void Dispose()
        {
            OnAttack = null;
        }
    }
}
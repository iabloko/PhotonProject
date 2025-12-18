using Core.Scripts.Game.CharacterLogic.CharacterCombat;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;

namespace Core.Scripts.Game.CharacterLogic.Simulation
{
    public sealed class CombatSimulation
    {
        private readonly ICharacterInput _input;
        private readonly IProjectSettings _projectSettings;
        private readonly CombatStateMachine _combat;

        public CombatSimulation(ICharacterInput input, IProjectSettings projectSettings, CombatStateMachine combat)
        {
            _input = input;
            _projectSettings = projectSettings;
            _combat = combat;
        }

        public void FixedTick()
        {
            if (_projectSettings.IsGamePaused) return;
            if (_input.AttackPressed) _combat.OnAttack();
        }
    }
}
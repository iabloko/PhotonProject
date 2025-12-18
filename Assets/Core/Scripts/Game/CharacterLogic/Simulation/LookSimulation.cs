using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;

namespace Core.Scripts.Game.CharacterLogic.Simulation
{
    public sealed class LookSimulation
    {
        private readonly ICharacterMotor _motor;
        private readonly ICharacterInput _input;
        private readonly IProjectSettings _projectSettings;

        public LookSimulation(ICharacterMotor motor, ICharacterInput input, IProjectSettings projectSettings)
        {
            _motor = motor;
            _input = input;
            _projectSettings = projectSettings;
        }

        public void FixedTick()
        {
            if (_projectSettings.IsGamePaused) return;
            _motor.AddLookRotation(_input.LookDelta);
        }
    }
}
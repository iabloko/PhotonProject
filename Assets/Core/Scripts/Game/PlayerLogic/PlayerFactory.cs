using Core.Scripts.Game.CharacterLogic;
using Core.Scripts.Game.CharacterLogic.CharacterCombat;
using Core.Scripts.Game.CharacterLogic.Presenter;
using Core.Scripts.Game.CharacterLogic.Simulation;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using Core.Scripts.Game.PlayerLogic.InputLogic;

namespace Core.Scripts.Game.PlayerLogic
{
    public sealed class PlayerFactory
    {
        private readonly IProjectSettings _projectSettings;

        private const int MAX_COMBO = 3;
        private const float COMBAT_RESET_SECONDS = 1.5f;

        public PlayerFactory(IProjectSettings projectSettings) => _projectSettings = projectSettings;

        public CharacterRuntime CreateRuntime(PlayerRuntimeConfig config, ICharacterMotor motor)
        {
            ITimeSource time = new RunnerTimeSource(config.Runner);

            CharacterAnimationPresenter anim = CreateAnimationPresenter(motor, config);
            CharacterEffectsPresenter effects = CreateEffectsPresenter(motor, config);
            SkinPresenter skin = new(config.VisualData);
            CharacterVisualPresenter visual = new(config.VisualData);
            WeaponPresenter weapons = new(config.WeaponData, anim);

            MoveSimulation moveSim = null;
            LookSimulation lookSim = null;
            CombatSimulation combatSim = null;
            CombatStateMachine combatState = null;

            if (config.HasStateAuthority)
            {
                ICharacterInput input = new PlayerInputAdapter(config.Input);

                combatState = CreateCombatStateMachine(config, time);
                lookSim = new LookSimulation(motor, input, _projectSettings);
                moveSim = new MoveSimulation(
                    motor,
                    input,
                    time,
                    config.GameplayData,
                    _projectSettings,
                    () => anim.PlayJump(true));

                combatSim = new CombatSimulation(input, _projectSettings, combatState);
            }

            return new CharacterRuntime(effects, anim, skin, visual, weapons, moveSim, lookSim, combatSim, combatState, motor);
        }

        private CharacterAnimationPresenter CreateAnimationPresenter(ICharacterMotor motor, PlayerRuntimeConfig config)
            => new(motor, config.Animator, _projectSettings, config.GameplayData);

        private CharacterEffectsPresenter CreateEffectsPresenter(ICharacterMotor motor, PlayerRuntimeConfig config)
            => new(motor, config.FootprintParticles, config.OnGroundParticles, config.GameplayData);

        private CombatStateMachine CreateCombatStateMachine(PlayerRuntimeConfig config, ITimeSource time)
        {
            return new CombatStateMachine(
                config.GetAttackSequence,
                config.SetAttackSequence,
                config.GetLastAttackTick,
                config.SetLastAttackTick,
                () => time.Tick,
                () => time.DeltaTime,
                maxCombo: MAX_COMBO,
                resetSeconds: COMBAT_RESET_SECONDS);
        }
    }
}

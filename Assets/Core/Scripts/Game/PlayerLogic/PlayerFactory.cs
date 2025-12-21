using Core.Scripts.Game.CharacterLogic;
using Core.Scripts.Game.CharacterLogic.Adapters;
using Core.Scripts.Game.CharacterLogic.CharacterCombat;
using Core.Scripts.Game.CharacterLogic.Presenter;
using Core.Scripts.Game.CharacterLogic.Simulation;
using Core.Scripts.Game.Infrastructure.Services.CinemachineService;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using Core.Scripts.Game.PlayerLogic.InputLogic;

namespace Core.Scripts.Game.PlayerLogic
{
    public sealed class PlayerFactory
    {
        private readonly ICinemachine _cinemachine;
        private readonly IProjectSettings _projectSettings;
        private const float CAMERA_ROTATION_SPEED = 2f;
        private const int MAX_COMBO = 3;
        private const float COMBAT_RESET_SECONDS = 0.5f;
        
        public PlayerFactory(ICinemachine cinemachine, IProjectSettings projectSettings)
        {
            _cinemachine = cinemachine;
            _projectSettings = projectSettings;
        }

        public CharacterRuntime CreateRuntime(PlayerRuntimeConfig config)
        {
            ICharacterMotor motor = new KccMotorAdapter(config.Kcc);
            ICharacterInput input = new PlayerInputAdapter(config.Input);
            ITimeSource time = new RunnerTimeSource(config.Runner);

            CharacterAnimationPresenter anim = CreateAnimationPresenter(motor, config);
            CharacterEffectsPresenter effects = CreateEffectsPresenter(motor, input, config);
            SkinPresenter skin = new(config.VisualData);
            CharacterVisualPresenter visual = new(config.VisualData);
            WeaponPresenter weapons = new(config.WeaponData, anim);
            CameraPresenter camera = CreateCameraPresenter(motor, input, config);

            MoveSimulation moveSim = null;
            LookSimulation lookSim = null;
            CombatSimulation combatSim = null;
            CombatStateMachine combatState = null;

            if (config.HasStateAuthority)
            {
                combatState = CreateCombatStateMachine(config, time);
                lookSim = new LookSimulation(motor, input, _projectSettings);
                moveSim = new MoveSimulation(motor, input, time, config.GameplayData, _projectSettings,
                    () => anim.PlayJump(true));
                combatSim = new CombatSimulation(input, _projectSettings, combatState);
            }

            return new CharacterRuntime(effects, anim, skin, visual, weapons, camera, moveSim, lookSim, combatSim, combatState);
        }

        private CharacterAnimationPresenter CreateAnimationPresenter(ICharacterMotor motor, PlayerRuntimeConfig config)
            => new(motor, config.Animator, _projectSettings, config.GameplayData);

        private CharacterEffectsPresenter CreateEffectsPresenter(ICharacterMotor motor, ICharacterInput input,
            PlayerRuntimeConfig config)
            => new(motor, input, config.FootprintParticles, config.OnGroundParticles);

        private CameraPresenter CreateCameraPresenter(ICharacterMotor motor, ICharacterInput input, PlayerRuntimeConfig config) 
            => new(motor, input, _cinemachine, _projectSettings, config.PreviewRotation, CAMERA_ROTATION_SPEED);

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
using System;
using Core.Scripts.Game.CharacterLogic;
using Core.Scripts.Game.CharacterLogic.Adapters;
using Core.Scripts.Game.CharacterLogic.CharacterCombat;
using Core.Scripts.Game.CharacterLogic.Data;
using Core.Scripts.Game.CharacterLogic.Presenter;
using Core.Scripts.Game.CharacterLogic.Simulation;
using Core.Scripts.Game.GamePlay.UsableItems;
using Fusion;
using Fusion.Addons.SimpleKCC;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Zenject;
using UniRx;
using Core.Scripts.Game.Infrastructure.ModelData;
using Core.Scripts.Game.Infrastructure.RequiresInjection;
using Core.Scripts.Game.Infrastructure.Services.CinemachineService;
using Core.Scripts.Game.Infrastructure.Services.Inventory;
using Core.Scripts.Game.Infrastructure.Services.NickName;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using Core.Scripts.Game.PlayerLogic.Input;
using Core.Scripts.Game.PlayerLogic.PlayerWeaponLogic;

namespace Core.Scripts.Game.PlayerLogic
{
    public sealed class Player : NetworkBehaviour, IAfterSpawned, IBeforeTick, IAfterTick, IRequiresInjection
    {
        public bool RequiresInjection { get; set; } = true;

        private const string PLAYER_LAYER = "Player";
        private const float COMBAT_RESET_SECONDS = .5f;

        [Title("Network Behaviour", subtitle: "", TitleAlignments.Right), Networked, UnitySerializeField]
        public NetworkString<_16> PlayerNickName { get; set; }

        [Networked, UnitySerializeField] public int CurrentHealth { get; set; }
        [Networked, UnitySerializeField] public CharacterVisualNetwork VisualNetwork { get; set; }
        [Networked, UnitySerializeField] public int PlayerWeaponId { get; set; }
        [Networked, UnitySerializeField] public int AttackSequence { get; set; }
        [Networked, UnitySerializeField] public int LastAttackTick { get; set; }

        [Title("Local Behavior", subtitle: "", TitleAlignments.Right), SerializeField]
        private CharacterVisual _characterVisualData;

        [SerializeField, TableList] private WeaponData[] _weaponData;
        [SerializeField] private TMP_Text _nickNameText;
        [SerializeField] private Material _playerMaterial;

        [SerializeField] private SimpleKCC _kcc;
        [SerializeField] private PlayerInput _input;
        [SerializeField] private GameplaySettings _gameplayData;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _previewRotation;

        [Title("Effects Behavior", subtitle: "", TitleAlignments.Right), SerializeField]
        private ParticleSystem _footprintParticles;
        [SerializeField] private ParticleSystem _onGroundParticles;

        private ICinemachine _cinemachine;
        private IProjectSettings _projectSettings;
        private IInventory _inventory;
        private INickNameFadeEffect _nickNameFadeEffect;

        private Camera _mainCamera;
        private ChangeDetector _changeDetector;

        private ICharacterMotor _motor;
        private ICharacterInput _charInput;
        private ITimeSource _time;

        private CharacterAnimationPresenter _anim;
        private CharacterEffectsPresenter _characterEffectsPresenter;
        private SkinPresenter _skinPresenter;
        private WeaponPresenter _weaponPresenter;

        private CameraPresenter _cameraPresenter;

        private MoveSimulation _moveSim;
        private LookSimulation _lookSim;
        private CombatStateMachine _combatState;
        private CombatSimulation _combatSim;
        private WeaponSelection _weaponSelection;
        private CharacterRuntime _runtime;

        private CompositeDisposable _compositeDisposable;

        [Inject]
        public void Constructor(ICinemachine c, IProjectSettings p, INickNameFadeEffect n, IInventory i)
        {
            _cinemachine = c;
            _projectSettings = p;
            _inventory = i;

            _mainCamera = Camera.main;
            _nickNameFadeEffect = n;
            _nickNameFadeEffect.Initialization(_mainCamera);
        }

        public override void Spawned()
        {
            base.Spawned();

            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

            _motor = new KccMotorAdapter(_kcc);
            _time = new RunnerTimeSource(Runner);
            _charInput = new PlayerInputAdapter(_input);

            CharacterAnimationPresenter anim = new(_motor, _animator, _projectSettings, _gameplayData);
            CharacterEffectsPresenter characterEffectsPresenter = new(_motor, _footprintParticles, _onGroundParticles);
            SkinPresenter skin = new(_characterVisualData);
            WeaponPresenter weapons = new(_weaponData, anim);

            CameraPresenter cameraPresenter = new(_motor, _charInput, _cinemachine, _projectSettings, _previewRotation, 2f);

            MoveSimulation moveSim = null;
            LookSimulation lookSim = null;
            CombatSimulation combatSim = null;
            CombatStateMachine combatState = null;

            if (Object.HasStateAuthority)
            {
                ChangeNetworkPlayerVisualData();
                ChangeNetworkPlayerNickName();
                ChangePlayerNicknameVisibility(false);
                
                _compositeDisposable = new CompositeDisposable();

                combatState = new CombatStateMachine(
                    () => AttackSequence, 
                    v => AttackSequence = v,
                    () => LastAttackTick, 
                    v => LastAttackTick = v,
                    () => _time.Tick, 
                    () => _time.DeltaTime,
                    maxCombo: 3, resetSeconds: COMBAT_RESET_SECONDS);

                lookSim = new LookSimulation(_motor, _charInput, _projectSettings);
                moveSim = new MoveSimulation(_motor, _charInput, _time, _gameplayData, _projectSettings,
                    () => anim.PlayJump(true));
                combatSim = new CombatSimulation(_charInput, _projectSettings, combatState);
                
                _weaponSelection = new WeaponSelection(_inventory, id => PlayerWeaponId = id);
            }

            _runtime = new CharacterRuntime(
                characterEffectsPresenter, anim, skin, weapons, cameraPresenter,
                moveSim, lookSim, combatSim, combatState);
        }


        public void AfterSpawned()
        {
            if (Object.HasInputAuthority)
            {
                _runtime.AfterSpawnedLocal();
            }
            else
            {
                _nickNameFadeEffect.RegisterNickName(_nickNameText);
            }
        }

        public void BeforeTick() => _runtime.BeforeTick();
        public void AfterTick() => _runtime.AfterTick();

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);

            _nickNameFadeEffect.UnregisterNickName(_nickNameText);

            _weaponSelection?.Dispose();
            _compositeDisposable?.Dispose();
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (Object.HasStateAuthority)
                _runtime.FixedTickSimulation();

            _runtime.FixedTickPresentation();
            _nickNameFadeEffect.FixedUpdateNetwork();
        }

        private void LateUpdate()
        {
            _runtime.LateTickPresentation();

            if (Object.HasInputAuthority)
                _runtime.LateTickLocal();
        }

        public override void Render()
        {
            foreach (string change in _changeDetector.DetectChanges(this, out _, out _))
            {
                switch (change)
                {
                    case nameof(PlayerNickName):
                        SetUpLocalPlayerNickName();
                        break;

                    case nameof(VisualNetwork):
                        _runtime.ApplySkin(VisualNetwork);
                        break;

                    case nameof(PlayerWeaponId):
                        _runtime.ApplyWeapon(PlayerWeaponId);
                        break;

                    case nameof(AttackSequence):
                        _runtime.ApplyAttackSequence(AttackSequence);
                        break;
                }
            }
        }

        private void SetUpLocalPlayerNickName()
        {
            try
            {
#if UNITY_EDITOR
                string playerName = string.Concat(PlayerNickName.Value, "_", Object.Id);
#elif !UNITY_EDITOR && UNITY_WEBGL
                string playerName = string.Concat(PlayerNickName.Value);
#endif
                _nickNameText.text = playerName;
                transform.name = playerName;
            }
            catch (Exception e)
            {
                Debug.LogError($"Player Room Enter Player ID {Object.Id} ERROR {e}");
                Debug.LogError($"Player Room Enter Player ID {Object.Id} ERROR {e.Message}");
            }
        }

        private void ChangeNetworkPlayerNickName()
        {
            var networkString = new NetworkString<_16>();
            networkString.Set(PLAYER_LAYER);
            PlayerNickName = networkString;
        }

        private void ChangeNetworkPlayerVisualData()
        {
            int hairId = UnityEngine.Random.Range(0, _characterVisualData.hair.Length);
            int headId = UnityEngine.Random.Range(0, _characterVisualData.heads.Length);
            int eyeId = UnityEngine.Random.Range(0, _characterVisualData.eyes.Length);
            int mouthId = UnityEngine.Random.Range(0, _characterVisualData.mouth.Length);
            int bodyId = UnityEngine.Random.Range(0, _characterVisualData.bodies.Length);

            VisualNetwork = new CharacterVisualNetwork(hairId, headId, eyeId, mouthId, bodyId);
        }
        
        private void ChangePlayerNicknameVisibility(bool status) => _nickNameText.gameObject.SetActive(status);
    }
}
using Core.Scripts.Game.CharacterLogic;
using Core.Scripts.Game.CharacterLogic.Data;
using Core.Scripts.Game.GamePlay.UsableItems;
using Core.Scripts.Game.Infrastructure.ModelData;
using Core.Scripts.Game.Infrastructure.RequiresInjection;
using Core.Scripts.Game.Infrastructure.Services.CinemachineService;
using Core.Scripts.Game.Infrastructure.Services.Inventory;
using Core.Scripts.Game.Infrastructure.Services.NickName;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using Core.Scripts.Game.PlayerLogic.InputLogic;
using Core.Scripts.Game.PlayerLogic.PlayerWeaponLogic;
using Fusion;
using Fusion.Addons.SimpleKCC;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game.PlayerLogic
{
    public sealed class Player : NetworkBehaviour, IAfterSpawned, IBeforeTick, IAfterTick, IRequiresInjection
    {
        public bool RequiresInjection { get; set; } = true;

        [Title("Network Behaviour"), Networked, UnitySerializeField]
        public NetworkString<_16> PlayerNickName { get; set; }

        [Networked, UnitySerializeField] public int CurrentHealth { get; set; }
        [Networked, UnitySerializeField] public CharacterVisualNetwork VisualNetwork { get; set; }
        [Networked, UnitySerializeField] public int PlayerWeaponId { get; set; }
        [Networked, UnitySerializeField] public int AttackSequence { get; set; }
        [Networked, UnitySerializeField] public int LastAttackTick { get; set; }
        
        [Title("Visual Data"), SerializeField] private CharacterVisual _characterVisualData;
        [SerializeField, TableList] private WeaponData[] _weaponData;
        [SerializeField] private TMP_Text _nickNameText;
        [SerializeField] private Material _playerMaterial;

        [Title("Components"), SerializeField] private SimpleKCC _kcc;
        [SerializeField] private PlayerInput _input;
        [SerializeField] private GameplaySettings _gameplayData;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _previewRotation;

        [Title("Effects"), SerializeField] private ParticleSystem _footprintParticles;
        [SerializeField] private ParticleSystem _onGroundParticles;
        
        private ICinemachine _cinemachine;
        private IProjectSettings _projectSettings;
        private INickNameFadeEffect _nickNameFadeEffect;
        private IInventory _inventory;
        private CharacterRuntime _runtime;
        private ChangeDetector _changeDetector;
        private CompositeDisposable _disposables;
        
        [Inject]
        public void Constructor(
            ICinemachine cinemachine, 
            IProjectSettings projectSettings, 
            INickNameFadeEffect nickNameFadeEffect,
            IInventory inventory)
        {
            _inventory = inventory;
            _cinemachine = cinemachine;
            _projectSettings = projectSettings;
            _nickNameFadeEffect = nickNameFadeEffect;
        }

        public override void Spawned()
        {
            base.Spawned();

            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            _disposables = new CompositeDisposable();
            _nickNameFadeEffect.Initialization(Camera.main);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);

            _runtime.Dispose();
            _disposables.Dispose();
            
            if (!Object.HasStateAuthority)
                _nickNameFadeEffect.UnregisterNickName(_nickNameText);
        }

        void IAfterSpawned.AfterSpawned()
        {
            InitializeRuntime();
            
            if (Object.HasStateAuthority)
            {
                _runtime.AfterSpawnedLocal();
                InitializeNetworkSystems();
            }
            else
            {
                _nickNameFadeEffect.RegisterNickName(_nickNameText);
                
                _runtime.ApplySkin(VisualNetwork);
                ApplyNickname();
            }
        }

        void IBeforeTick.BeforeTick()
        {
            _runtime.BeforeTick();
        }

        void IAfterTick.AfterTick()
        {
            _runtime.AfterTick();
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (Object.HasStateAuthority)
            {
                _runtime.FixedTickSimulation();
                _nickNameFadeEffect.FixedUpdateNetwork();
            }

            _runtime.FixedTickPresentation();
        }

        public override void Render()
        {
            foreach (string change in _changeDetector.DetectChanges(this, out _, out _))
            {
                switch (change)
                {
                    case nameof(PlayerNickName):
                        ApplyNickname();
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
        
        private void InitializeRuntime()
        {
            PlayerRuntimeConfig config = CreateRuntimeConfig();
            PlayerFactory factory = new(_cinemachine, _projectSettings);
            _runtime = factory.CreateRuntime(config);
        }

        private void InitializeNetworkSystems()
        {
            CurrentHealth = 100;
            
            VisualNetwork = _runtime.CreateRandomVisual();
            PlayerNickName = _runtime.CreateDefaultNickname();
            _nickNameText.gameObject.SetActive(false);
            
            WeaponSelection weaponSelection = new(_inventory, id => PlayerWeaponId = id);
            _disposables.Add(weaponSelection);
        }

        private PlayerRuntimeConfig CreateRuntimeConfig()
        {
            return new PlayerRuntimeConfig(
                kcc: _kcc,
                input: _input,
                animator: _animator,
                previewRotation: _previewRotation,
                footprintParticles: _footprintParticles,
                onGroundParticles: _onGroundParticles,
                visualData: _characterVisualData,
                weaponData: _weaponData,
                gameplayData: _gameplayData,
                runner: Runner,
                hasStateAuthority: Object.HasStateAuthority,
                getAttackSequence: () => AttackSequence,
                setAttackSequence: v => AttackSequence = v,
                getLastAttackTick: () => LastAttackTick,
                setLastAttackTick: v => LastAttackTick = v
            );
        }

        private void LateUpdate()
        {
            _runtime.LateTickPresentation();

            if (Object.HasStateAuthority)
                _runtime.LateTickLocal();
        }

        private void ApplyNickname()
        {
            try
            {
                string formattedName = _runtime.FormatNickname(PlayerNickName.Value, Object.Id);
                _nickNameText.text = formattedName;
                transform.name = formattedName;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to apply nickname for player {Object.Id}: {e.Message}");
            }
        }
    }
}
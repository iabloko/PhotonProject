using Core.Scripts.Game.CharacterLogic;
using Core.Scripts.Game.CharacterLogic.Data;
using Core.Scripts.Game.Constants;
using Core.Scripts.Game.GamePlay.UsableItems;
using Core.Scripts.Game.Infrastructure.ModelData;
using Core.Scripts.Game.Infrastructure.RequiresInjection;
using Core.Scripts.Game.Infrastructure.Services.NickName;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using Core.Scripts.Game.PlayerLogic.InputLogic;
using Fusion;
using Fusion.Addons.SimpleKCC;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game.PlayerLogic
{
    public interface IItemPickUpHandler
    {
        void TryPickUp(Weapon pickUpItem);
    }

    public sealed class Player : NetworkBehaviour, IAfterSpawned, IBeforeTick, IAfterTick, IRequiresInjection,
        IItemPickUpHandler
    {
        public bool RequiresInjection { get; set; } = true;

        [Title("Network Behaviour"), Networked, UnitySerializeField]
        public NetworkString<_16> PlayerNickName { get; set; }

        [Networked, UnitySerializeField] public int CurrentHealth { get; set; }
        [Networked, UnitySerializeField] public int PlayerWeaponId { get; set; }
        [Networked, UnitySerializeField] public int AttackSequence { get; set; }
        [Networked, UnitySerializeField] public int LastAttackTick { get; set; }
        [Networked, UnitySerializeField] public CharacterVisualNetwork VisualNetwork { get; set; }

        [Title("Visual Data"), SerializeField] private CharacterVisual _characterVisualData;
        [SerializeField, TableList] private WeaponData[] _weaponData;
        [SerializeField] private TMP_Text _nickNameText;
        [SerializeField] private Material _playerMaterial;

        [Title("Components"), SerializeField] private SimpleKCC _kcc;
        [SerializeField] private PlayerInput _input; // DO NOT TOUCH (per request)
        [SerializeField] private GameplaySettings _gameplayData;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _previewRotation;

        [Title("Effects"), SerializeField] private ParticleSystem _footprintParticles;
        [SerializeField] private ParticleSystem _onGroundParticles;

        [Title("Local Only"), SerializeField] private PlayerLocalAddon _localAddonPrefab;

        private IProjectSettings _projectSettings;
        private INickNameFadeEffect _nickNameFadeEffect;
        private DiContainer _container;

        private PlayerLocalAddon _localAddonInstance;

        private CharacterRuntime _runtime;
        private ChangeDetector _changeDetector;

        [Inject]
        public void Constructor(
            IProjectSettings projectSettings,
            INickNameFadeEffect nickNameFadeEffect,
            DiContainer container)
        {
            _projectSettings = projectSettings;
            _nickNameFadeEffect = nickNameFadeEffect;
            _container = container;
        }

        public override void Spawned()
        {
            base.Spawned();
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        }

        void IAfterSpawned.AfterSpawned()
        {
            InitializeRuntime();

            if (Object.HasInputAuthority)
            {
                // _kcc.SetColliderLayer(LayerMask.NameToLayer(GameConstants.LOCAL_PLAYER));
                // _kcc.Collider.transform.tag = GameConstants.LOCAL_PLAYER;

                TryCreateLocalAddon();
                InitializeNetworkSystems();
            }
            else
            {
                // _kcc.SetColliderLayer(LayerMask.NameToLayer(GameConstants.REMOTE_PLAYER));
                // _kcc.Collider.transform.tag = GameConstants.REMOTE_PLAYER;

                _nickNameFadeEffect.RegisterNickName(_nickNameText);

                _runtime.ApplySkin(VisualNetwork);
                _runtime.ApplyWeapon(PlayerWeaponId);
                _runtime.ApplyAttackSequence(AttackSequence);
                ApplyNickname();
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);

            if (!Object.HasInputAuthority)
                _nickNameFadeEffect.UnregisterNickName(_nickNameText);

            if (_localAddonInstance != null)
            {
                Destroy(_localAddonInstance.gameObject);
                _localAddonInstance = null;
            }

            _runtime?.Dispose();
            _runtime = null;
        }

        void IBeforeTick.BeforeTick()
        {
            _runtime?.BeforeTick();
        }

        void IAfterTick.AfterTick()
        {
            _runtime.AfterTick();
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (Object.HasStateAuthority)
                _runtime.FixedTickSimulation();

            _runtime.FixedTickPresentation();
        }

        private void LateUpdate()
        {
            _runtime.LateTickPresentation();
        }

        public override void Render()
        {
            if (_runtime == null) return;

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

        void IItemPickUpHandler.TryPickUp(Weapon pickUpItem)
        {
            if (Object.HasStateAuthority)
                PlayerWeaponId = pickUpItem.id;
        }

        private void TryCreateLocalAddon()
        {
            _localAddonInstance =
                _container.InstantiatePrefabForComponent<PlayerLocalAddon>(_localAddonPrefab, transform);
            _localAddonInstance.Bind(_kcc, _input, _previewRotation);
            _localAddonInstance.transform.SetParent(transform);
        }

        private void InitializeRuntime()
        {
            PlayerRuntimeConfig config = CreateRuntimeConfig();
            PlayerFactory factory = new(_projectSettings);
            _runtime = factory.CreateRuntime(config);
        }

        private void InitializeNetworkSystems()
        {
            CurrentHealth = 100;

            VisualNetwork = _runtime.CreateRandomVisual();
            PlayerNickName = _runtime.CreateDefaultNickname();
            _nickNameText.gameObject.SetActive(false);
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
                setAttackSequence: value => AttackSequence = value,
                getLastAttackTick: () => LastAttackTick,
                setLastAttackTick: value => LastAttackTick = value);
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
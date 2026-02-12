using System;
using Core.Scripts.Game.CharacterLogic;
using Core.Scripts.Game.CharacterLogic.Adapters;
using Core.Scripts.Game.CharacterLogic.Data;
using Core.Scripts.Game.Combat.Data;
using Core.Scripts.Game.Combat.Events;
using Core.Scripts.Game.Combat.Presenters;
using Core.Scripts.Game.Constants;
using Core.Scripts.Game.GamePlay.UsableItems;
using Core.Scripts.Game.Infrastructure.ModelData;
using Core.Scripts.Game.Infrastructure.RequiresInjection;
using Core.Scripts.Game.Infrastructure.Services.NickName;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using Core.Scripts.Game.PlayerLogic.InputLogic;
using Core.Scripts.Game.ScriptableObjects.Configs.Logger;
using Fusion;
using Fusion.Addons.SimpleKCC;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Zenject;
using LogLevel = Core.Scripts.Game.ScriptableObjects.Configs.Logger.LogLevel;

namespace Core.Scripts.Game.PlayerLogic
{
    public interface IItemPickUpHandler
    {
        void TryPickUp(Weapon pickUpItem);
    }

    public sealed class Player : NetworkBehaviour, IAfterSpawned, IBeforeTick, IAfterTick, IRequiresInjection,
        IItemPickUpHandler, IDamageable
    {
        public bool RequiresInjection { get; set; } = true;

        [Title("Network Behaviour"), Networked, UnitySerializeField]
        public NetworkString<_16> PlayerNickName { get; set; }
        [Networked, UnitySerializeField] public int PlayerWeaponId { get; set; }
        [Networked, UnitySerializeField] public int AttackSequence { get; set; }
        [Networked, UnitySerializeField] private int LastAttackTick { get; set; }

        [Title("Network Behaviour", "Health"), Networked, UnitySerializeField, HideLabel]
        public HealthNetwork Health { get; set; }
        [Title("Network Behaviour", "Visual"), Networked, UnitySerializeField,]
        public CharacterVisualNetwork VisualNetwork { get; set; }

        NetworkId IDamageable.NetworkId => Object.Id;
        Transform IDamageable.Transform => transform;

        HealthNetwork IDamageable.Health => Health;
        bool IDamageable.IsDead => Health.IsDead;
        int IDamageable.GetArmor() => Health.armor;

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
        [SerializeField] private ParticleSystem _hitParticles;

        // [Title("Combat UI"), SerializeField]
        // private Image _healthBar;
        // [SerializeField] private CanvasGroup _damageFlash;

        [Title("Local Only"), SerializeField] private PlayerLocalAddon _localAddonPrefab;

        private IProjectSettings _projectSettings;
        private INickNameFadeEffect _nickNameFadeEffect;
        private DiContainer _container;
        private ICharacterMotor _motor;
        private RunnerTimeSource _time;
        private GameLogger _logger;

        private PlayerLocalAddon _local;

        private CharacterRuntime _runtime;
        private CombatRuntime _combatRuntime;
        private ChangeDetector _changeDetector;
        private NetworkId _networkId;
        private Transform _transform;
        private bool _isDead;
        private IWeaponRegistry _weaponRegistry;

        private readonly int _maxHealth = 100;

        [Inject]
        public void Constructor(
            IProjectSettings projectSettings,
            INickNameFadeEffect nickNameFadeEffect,
            IWeaponRegistry weaponRegistry,
            DiContainer container,
            GameLogger logger)
        {
            _logger = logger;
            _projectSettings = projectSettings;
            _nickNameFadeEffect = nickNameFadeEffect;
            _weaponRegistry = weaponRegistry;
            _container = container;
        }

        public override void Spawned()
        {
            base.Spawned();
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        }

        void IAfterSpawned.AfterSpawned()
        {
            _motor = new KccMotorAdapter(_kcc);
            _time = new RunnerTimeSource(Runner);

            InitializeRuntime();
            InitializeCombat();

            if (Object.HasInputAuthority)
            {
                ChangeTag(GameConstants.LOCAL_PLAYER);

                TryCreateLocalAddon(_motor);
                InitializeNetworkSystems();
            }
            else
            {
                ChangeTag(GameConstants.REMOTE_PLAYER);

                _runtime.ApplySkin(VisualNetwork);
                _runtime.ApplyWeapon(PlayerWeaponId);
                _runtime.ApplyAttackSequence(AttackSequence);
                _nickNameFadeEffect.RegisterNickName(_nickNameText);
                ApplyNickname();
            }
        }

        private void ChangeTag(string value) => _runtime.SetColliderTag(value);

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);

            if (!Object.HasInputAuthority)
                _nickNameFadeEffect.UnregisterNickName(_nickNameText);

            if (_local != null)
            {
                Destroy(_local.gameObject);
                _local = null;
            }

            _combatRuntime?.Dispose();
            _runtime?.Dispose();
            _combatRuntime = null;
            _runtime = null;
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

            if (Health.IsDead) return;

            if (Object.HasStateAuthority)
                _runtime.FixedTickSimulation();

            _runtime.FixedTickPresentation();
        }

        private void LateUpdate()
        {
            _runtime.LateTickPresentation();
            _combatRuntime.LateUpdate(Time.deltaTime, Health.NormalizedHealth);
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

                    case nameof(Health):
                        OnHealthChanged();
                        break;
                }
            }
        }

        private void ExecuteAttack()
        {
            if (!Object.HasStateAuthority) return;
            if (Health.IsDead) return;

            _combatRuntime.TryAttack();
        }

        public void Heal(int amount)
        {
            if (!Object.HasStateAuthority) return;
            _combatRuntime.HealthSim.Heal(amount);
        }

        private void Respawn()
        {
            if (!Object.HasStateAuthority) return;

            _combatRuntime.HealthSim.Reset();
            _combatRuntime.DeathPresenter.Reset();
        }

        void IItemPickUpHandler.TryPickUp(Weapon pickUpItem)
        {
            if (Object.HasStateAuthority)
                PlayerWeaponId = pickUpItem.id;
        }

        private void TryCreateLocalAddon(ICharacterMotor motor)
        {
            _local = _container.InstantiatePrefabForComponent<PlayerLocalAddon>(_localAddonPrefab, transform);
            _local.transform.SetParent(transform);
            _local.Bind(motor, _input, _previewRotation);
        }

        private void InitializeRuntime()
        {
            PlayerRuntimeConfig config = CreateRuntimeConfig();
            PlayerFactory factory = new(_projectSettings);
            _runtime = factory.CreateRuntime(config, _motor, _time, onAttackExecuted: ExecuteAttack);
        }

        private void InitializeCombat()
        {
            if (Object.HasStateAuthority)
            {
                Health = new HealthNetwork(_maxHealth, 10);
            }

            CombatConfig config = new()
            {
                Motor = _motor,
                Time = _time,
                WeaponRegistry = _weaponRegistry,

                GetWeaponId = () => PlayerWeaponId,
                GetNetworkId = () => Object.Id,
                GetOwnLayer = () => gameObject.layer,

                GetHealth = () => Health,
                SetHealth = value => Health = value,

                HasStateAuthority = Object.HasStateAuthority,

                Animator = _animator,
                HitParticles = _hitParticles,
                // HealthBar = _healthBar,
                // DamageFlash = _damageFlash,

                OnDamageDealt = OnDamageDealt,
                OnDamageReceived = OnDamageReceived,
                OnDeath = OnDeath
            };

            CombatFactory factory = new(
                damageableLayer: LayerMask.NameToLayer(GameConstants.PLAYER));

            _combatRuntime = factory.Create(config);
        }

        private void InitializeNetworkSystems()
        {
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
            catch (Exception e)
            {
                _logger.Log<Player>(LogLevel.Error, $"Failed to apply nickname for player {Object.Id}: {e.Message}");
            }
        }

        void IDamageable.ApplyDamage(DamageEvent damageEvent)
        {
            _logger.Log<Player>(LogLevel.Info,
                $"[Player {Object.Id}] ApplyDamage {damageEvent.Result.FinalDamage} damage from {damageEvent.AttackerId} : {Object.HasStateAuthority}");

            if (Object.HasStateAuthority)
            {
                ApplyDamageInternal(damageEvent);
            }
            else
            {
                RPC_RequestDamage(damageEvent);
            }
        }
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_RequestDamage(DamageEvent damage) => ApplyDamageInternal(damage);

        private void ApplyDamageInternal(DamageEvent damageEvent)
        {
            HealthNetwork currentHealth = Health;
            currentHealth.current = Mathf.Clamp(currentHealth.current - damageEvent.Result.FinalDamage, 0, _maxHealth);
            Health = currentHealth;
        }

        private void OnDamageDealt(DamageEvent damageEvent)
        {
            _logger.Log<Player>(LogLevel.Info,
                $"[Player {Object.Id}] Dealt {damageEvent.Result.FinalDamage} damage to {damageEvent.VictimId}");
            // _hitParticles.Play();
        }

        private void OnDamageReceived(DamageEvent damageEvent)
        {
            _logger.Log<Player>(LogLevel.Info,
                $"[Player {Object.Id}] Received {damageEvent.Result.FinalDamage} damage from {damageEvent.AttackerId}");
        }

        private void OnDeath()
        {
            _logger.Log<Player>(LogLevel.Info, $"[Player {Object.Id}] Died!");
            _combatRuntime.PlayDeath();
        }

        private void OnHealthChanged()
        {
            _logger.Log<Player>(LogLevel.Info, $"[Player {Object.Id}] On Health Changed!");
            _hitParticles.Play();

            _combatRuntime.SetHealth(Health.NormalizedHealth);
            if (Health.IsDead) _combatRuntime.PlayDeath();
        }
    }
}
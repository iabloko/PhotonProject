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
using Fusion;
using Fusion.Addons.SimpleKCC;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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
        [Networked, UnitySerializeField] public int LastAttackTick { get; set; }
        
        [Title("Network Behaviour", "Health"), Networked, UnitySerializeField, HideLabel] 
        public HealthNetwork Health { get; set; }
        [Title("Network Behaviour", "Visual"), Networked, UnitySerializeField,] 
        public CharacterVisualNetwork VisualNetwork { get; set; }

        NetworkId IDamageable.NetworkId => Object.Id;
        HealthNetwork IDamageable.Health => Health;
        Transform IDamageable.Transform => transform;
        bool IDamageable.IsDead => Health.IsDead;

        void IDamageable.ApplyDamage(DamageEvent damageEvent)
        {
            if (!Object.HasStateAuthority) return;
            _combatRuntime?.HealthSim?.ApplyDamage(damageEvent);
        }

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

        private PlayerLocalAddon _local;

        private CharacterRuntime _runtime;
        private CombatRuntime _combatRuntime;
        private ChangeDetector _changeDetector;
        private NetworkId _networkId;
        private Transform _transform;
        private bool _isDead;
        private IWeaponRegistry _weaponRegistry;

        [Inject]
        public void Constructor(
            IProjectSettings projectSettings,
            INickNameFadeEffect nickNameFadeEffect,
            IWeaponRegistry weaponRegistry,
            DiContainer container)
        {
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
                Health = new HealthNetwork(100, 10);
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
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to apply nickname for player {Object.Id}: {e.Message}");
            }
        }


        private void OnDamageDealt(DamageEvent damageEvent)
        {
            Debug.Log($"[Player {Object.Id}] Dealt {damageEvent.Result.FinalDamage} damage to {damageEvent.VictimId}");
        }

        private void OnDamageReceived(DamageEvent damageEvent)
        {
            Debug.Log(
                $"[Player {Object.Id}] Received {damageEvent.Result.FinalDamage} damage from {damageEvent.AttackerId}");
            // _combatRuntime.HealthPresenter.OnDamageReceived();
        }

        private void OnDeath()
        {
            Debug.Log($"[Player {Object.Id}] Died!");
            _combatRuntime.PlayDeath();
        }

        private void OnHealthChanged()
        {
            Debug.Log($"[Player {Object.Id}] On Health Changed!");
            
            _combatRuntime.SetHealth(Health.NormalizedHealth);
            if (Health.IsDead) _combatRuntime.PlayDeath();
        }
    }
}
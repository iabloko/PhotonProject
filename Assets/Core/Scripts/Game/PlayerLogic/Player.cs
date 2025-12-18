using System;
using Fusion;
using Fusion.Addons.SimpleKCC;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Zenject;
using Core.Scripts.Game.Infrastructure.ModelData;
using Core.Scripts.Game.Infrastructure.RequiresInjection;
using Core.Scripts.Game.Infrastructure.Services.CinemachineService;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using Core.Scripts.Game.PlayerLogic.Combat;
using Core.Scripts.Game.PlayerLogic.ContextLogic;
using Core.Scripts.Game.PlayerLogic.Inventory;
using Core.Scripts.Game.PlayerLogic.Movement;
using Core.Scripts.Game.PlayerLogic.NetworkInput;
using Core.Scripts.Game.PlayerLogic.Visual;
using Core.Scripts.Game.ScriptableObjects.Items;
using UniRx;
using Animation = Core.Scripts.Game.PlayerLogic.Movement.Animation;
using Random = UnityEngine.Random;

namespace Core.Scripts.Game.PlayerLogic
{
    public sealed class Player : NetworkBehaviour, IAfterSpawned, IBeforeTick, IAfterTick, IRequiresInjection
    {
        public bool RequiresInjection { get; set; } = true;

        [Title("Network Behaviour", subtitle: "", TitleAlignments.Right), Networked, UnitySerializeField]
        public NetworkString<_16> PlayerNickName { get; set; }
        [Networked, UnitySerializeField] public int CurrentHealth { get; set; }
        [Networked, UnitySerializeField] public PlayerVisualNetwork VisualNetwork { get; set; }
        [Networked, UnitySerializeField] public int PlayerWeaponId { get; set; }
        [Networked, UnitySerializeField] public int AttackSequence { get; set; }
        [Networked, UnitySerializeField] public int LastAttackTick { get; set; }

        [Title("Local Behavior", subtitle: "", TitleAlignments.Right), SerializeField]
        private PlayerVisual _playerVisualData;
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
        
        private const float COMBAT_RESET_SECONDS = .5f;

        private ICinemachine _cinemachine;
        private IProjectSettings _projectSettings;
        private IPlayerInventory _inventory;
        private INickNameFadeEffect _nickNameFadeEffect;

        private PlayerContext _ctx;
        private Animation _anim;
        private Rotation _rotation;
        private Effects _effects;
        private Moving _moving;
        private ChangeDetector _changeDetector;
        private CompositeDisposable _disposables;
        private PlayerCombat _combat;

        private Camera _mainCamera;

        private const string PLAYER_LAYER = "Player";

        #region ZENJECT

        [Inject]
        public void Constructor(ICinemachine c, IProjectSettings p, INickNameFadeEffect n, IPlayerInventory i)
        {
            _cinemachine = c;
            _projectSettings = p;
            _inventory = i;

            _mainCamera = Camera.main;
            _nickNameFadeEffect = n;
            _nickNameFadeEffect.Initialization(_mainCamera);
        }

        #endregion

        #region NETWORK_BEHAVIOR

        public override void Spawned()
        {
            base.Spawned();

            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            _ctx = new PlayerContext(_kcc, _input, _gameplayData, _projectSettings, Runner, Object.HasStateAuthority);
            _effects = new Effects(_footprintParticles, _onGroundParticles, _ctx);

            if (Object.HasStateAuthority)
            {
                ChangeNetworkPlayerVisualData();
                ChangeNetworkPlayerNickName();

                _anim = new Animation(_ctx, _animator, _projectSettings, _gameplayData);
                _combat = new PlayerCombat(_ctx, _projectSettings, CombatLogic);
                _rotation = new Rotation(_ctx, _cinemachine, _projectSettings, _previewRotation, 2f);
                _moving = new Moving(_ctx, _projectSettings, JumpAnimation);

                _disposables = new CompositeDisposable();
                _inventory.CurrentWeapon
                    .Where(w => w != null)
                    .Subscribe(SetWeaponInHand)
                    .AddTo(_disposables);
            }
            else
            {
                _nickNameFadeEffect.RegisterNickName(_nickNameText);
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);

            _nickNameFadeEffect.UnregisterNickName(_nickNameText);

            if (Object.HasInputAuthority)
            {
                _disposables?.Dispose();
                _combat.Dispose();
            }
        }

        public override void Render()
        {
            foreach (string change in
                     _changeDetector.DetectChanges(this,
                         out NetworkBehaviourBuffer previous,
                         out NetworkBehaviourBuffer current))
            {
                switch (change)
                {
                    case nameof(PlayerNickName):
                        SetUpLocalPlayerNickName();
                        break;
                    case nameof(CurrentHealth):
                        break;
                    case nameof(VisualNetwork):
                        SkinChanged();
                        break;
                    case nameof(PlayerWeaponId):
                        WeaponChanged();
                        break;
                    case nameof(AttackSequence):
                        AttackSequenceChanged();
                        break;
                }
            }
        }

        void IAfterSpawned.AfterSpawned()
        {
            ChangePlayerNicknameVisibility(!Object.HasStateAuthority);

            if (Object.HasInputAuthority)
            {
                _rotation.AfterSpawned();
                _moving.AfterSpawned();
            }
            else
            {
                SkinChanged();
            }
        }

        void IBeforeTick.BeforeTick()
        {
            _effects.BeforeTick();
        }

        void IAfterTick.AfterTick()
        {
            if (Object.HasStateAuthority)
                _moving.AfterTick();
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (Object.HasStateAuthority)
            {
                _rotation.FixedUpdateNetwork();
                _moving.FixedUpdateNetwork();
                _combat.FixedUpdateNetwork();
                CombatTimer_TickBased();
            }

            _effects.OnGroundEffect();
            _nickNameFadeEffect.FixedUpdateNetwork();
        }
        
        private void CombatTimer_TickBased()
        {
            if (AttackSequence == 0) return;
            
            int ticksForReset = Mathf.CeilToInt(COMBAT_RESET_SECONDS / Runner.DeltaTime);

            int now = Runner.Tick;
            int last = LastAttackTick;

            if ((now - last) >= ticksForReset) AttackSequence = 0;
        }

        #endregion

        private void LateUpdate()
        {
            _effects.LateUpdate();

            if (!Object.HasInputAuthority) return;

            _rotation.LateUpdate();
            _anim.LateUpdate();
        }

        private void CombatLogic()
        {
            LastAttackTick = Runner.Tick;
            ChangeAttackSequence();
        }

        private void ChangeAttackSequence()
        {
            int currentAttack = AttackSequence;
            currentAttack++;
            currentAttack = currentAttack > 3 ? 1 : currentAttack;
            AttackSequence = currentAttack;
        }

        private void ResetAttackSequence() => AttackSequence = 0;

        private void JumpAnimation() => _anim.PlayJump(true);

        private void ChangeNetworkPlayerNickName()
        {
            var networkString = new NetworkString<_16>();
            networkString.Set(PLAYER_LAYER);

            PlayerNickName = networkString;
            transform.name = PLAYER_LAYER;
        }

        private void ChangeNetworkPlayerVisualData()
        {
            int hairId = Random.Range(0, _playerVisualData.hair.Length - 1);
            int headId = Random.Range(0, _playerVisualData.heads.Length - 1);
            int eyeId = Random.Range(0, _playerVisualData.eyes.Length - 1);
            int mouthId = Random.Range(0, _playerVisualData.mouth.Length - 1);
            int bodyId = Random.Range(0, _playerVisualData.bodies.Length - 1);

            VisualNetwork = new PlayerVisualNetwork(hairId, headId, eyeId, mouthId, bodyId);
        }

        private void SkinChanged()
        {
            for (int i = 0; i < _playerVisualData.hair.Length; i++)
                _playerVisualData.hair[i].SetActive(i == VisualNetwork.hairID);

            for (int i = 0; i < _playerVisualData.heads.Length; i++)
                _playerVisualData.heads[i].SetActive(i == VisualNetwork.headID);

            for (int i = 0; i < _playerVisualData.eyes.Length; i++)
                _playerVisualData.eyes[i].SetActive(i == VisualNetwork.eyeID);

            for (int i = 0; i < _playerVisualData.mouth.Length; i++)
                _playerVisualData.mouth[i].SetActive(i == VisualNetwork.mountID);

            for (int i = 0; i < _playerVisualData.bodies.Length; i++)
                _playerVisualData.bodies[i].SetActive(i == VisualNetwork.bodyID);
        }

        private void ChangePlayerNicknameVisibility(bool status) => _nickNameText.gameObject.SetActive(status);

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

        private void WeaponChanged()
        {
            for (int i = 0; i < _weaponData.Length; i++)
            {
                if (_weaponData[i].weaponConfig.id != PlayerWeaponId) continue;
                EnableWeapon(_weaponData[i]);
                ChangeAnimatorController(_weaponData[i].weaponConfig.weaponAnimations);
                break;
            }
        }

        private void AttackSequenceChanged()
        {
            bool inCombat = AttackSequence != 0;
            _anim.SetCombatStatus(inCombat);
            _anim.SetAttackAnimation(AttackSequence);
        }

        private void EnableWeapon(WeaponData data)
        {
            for (int i = 0; i < data.visuals.Length; i++)
            {
                data.visuals[i].prefab.SetActive(true);
            }
        }

        private void SetWeaponInHand(Weapon weapon)
        {
            if (Object.HasStateAuthority)
            {
                PlayerWeaponId = weapon.id;
            }
        }

        private void ChangeAnimatorController(AnimatorOverrideController controller)
        {
            _anim.OverrideAnimatorController(controller);
        }
    }
}
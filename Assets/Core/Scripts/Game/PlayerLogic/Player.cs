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
using Core.Scripts.Game.PlayerLogic.ContextLogic;
using Core.Scripts.Game.PlayerLogic.Movement;
using Core.Scripts.Game.PlayerLogic.NetworkInput;
using Core.Scripts.Game.PlayerLogic.Visual;

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

        [Title("Local Behavior", subtitle: "", TitleAlignments.Right), SerializeField]
        private PlayerVisual playerVisualData;
        [SerializeField] private TMP_Text nickNameText;
        [SerializeField] private Material playerMaterial;
        [SerializeField] private SimpleKCC kcc;
        [SerializeField] private PlayerInput input;
        [SerializeField] private RoomSettings roomData;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform previewRotation;
        [SerializeField] private ParticleSystem footprintParticles;
        [SerializeField] private ParticleSystem onGroundParticles;

        private Camera _mainCamera;
        private ICinemachine _cinemachine;
        private IProjectSettings _projectSettings;

        private PlayerContext _ctx;
        private Animation _anim;
        private Rotation _rotation;
        private Effects _effects;
        private Moving _moving;
        private INickNameFadeEffect _nickNameFadeEffect;
        private ChangeDetector _changeDetector;

        private const string PLAYER_LAYER = "Player";

        #region ZENJECT

        [Inject]
        public void Constructor(
            ICinemachine cinemachine, IProjectSettings projectSettings, INickNameFadeEffect nickNameFadeEffect)
        {
            Debug.Log($"[Player] Constructor {cinemachine} | {projectSettings} | {nickNameFadeEffect}");
            _cinemachine = cinemachine;
            _projectSettings = projectSettings;

            _mainCamera = Camera.main;
            _nickNameFadeEffect = nickNameFadeEffect;
            _nickNameFadeEffect.Initialization(_mainCamera);
        }

        #endregion

        #region NETWORK_BEHAVIOR

        public override void Spawned()
        {
            base.Spawned();

            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            _ctx = new PlayerContext(kcc, input, roomData, _projectSettings, Runner, Object.HasStateAuthority);
            _effects = new Effects(footprintParticles, onGroundParticles, _ctx);

            if (Object.HasStateAuthority)
            {
                ChangeNetworkPlayerVisualData();
                ChangeNetworkPlayerNickName();

                _anim = new Animation(_ctx, animator, _projectSettings, roomData);
                _rotation = new Rotation(_ctx, _cinemachine, _projectSettings, previewRotation, 2f);
                _moving = new Moving(_ctx, _projectSettings, JumpAnimation);
            }
            else
            {
                _nickNameFadeEffect.RegisterNickName(nickNameText);
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);

            _nickNameFadeEffect.UnregisterNickName(nickNameText);

            if (Object.HasInputAuthority)
                _rotation.Dispose();
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
            }
            
            _effects.OnGroundEffect();
            _nickNameFadeEffect.FixedUpdateNetwork();
        }

        #endregion

        private void LateUpdate()
        {
            _effects.LateUpdate();
            
            if (!Object.HasInputAuthority) return;
            
            _rotation.LateUpdate();
            _anim.LateUpdate();
        }

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
            int hairId = Random.Range(0, playerVisualData.hair.Length - 1);
            int headId = Random.Range(0, playerVisualData.heads.Length - 1);
            int eyeId = Random.Range(0, playerVisualData.eyes.Length - 1);
            int mouthId = Random.Range(0, playerVisualData.mouth.Length - 1);
            int bodyId = Random.Range(0, playerVisualData.bodies.Length - 1);

            VisualNetwork = new PlayerVisualNetwork(hairId, headId, eyeId, mouthId, bodyId);
        }

        private void SkinChanged()
        {
            for (int i = 0; i < playerVisualData.hair.Length; i++)
                playerVisualData.hair[i].SetActive(i == VisualNetwork.hairID);

            for (int i = 0; i < playerVisualData.heads.Length; i++)
                playerVisualData.heads[i].SetActive(i == VisualNetwork.headID);

            for (int i = 0; i < playerVisualData.eyes.Length; i++)
                playerVisualData.eyes[i].SetActive(i == VisualNetwork.eyeID);

            for (int i = 0; i < playerVisualData.mouth.Length; i++)
                playerVisualData.mouth[i].SetActive(i == VisualNetwork.mountID);

            for (int i = 0; i < playerVisualData.bodies.Length; i++)
                playerVisualData.bodies[i].SetActive(i == VisualNetwork.bodyID);
        }

        private void ChangePlayerNicknameVisibility(bool status) => nickNameText.gameObject.SetActive(status);

        private void SetUpLocalPlayerNickName()
        {
            try
            {
#if UNITY_EDITOR
                string playerName = string.Concat(PlayerNickName.Value, "_", Object.Id);
#elif !UNITY_EDITOR && UNITY_WEBGL
                string playerName = string.Concat(PlayerNickName.Value);
#endif
                nickNameText.text = playerName;
                transform.name = playerName;
            }
            catch (Exception e)
            {
                Debug.LogError($"Player Room Enter Player ID {Object.Id} ERROR {e}");
                Debug.LogError($"Player Room Enter Player ID {Object.Id} ERROR {e.Message}");
            }
        }
    }
}
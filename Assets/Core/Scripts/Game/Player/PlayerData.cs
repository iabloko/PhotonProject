using System.Threading;
using Core.Scripts.Game.Infrastructure.Services.Cinemachine;
using Core.Scripts.Game.Infrastructure.Services.GamePool;
using Core.Scripts.Game.Player.Locomotion;
using Core.Scripts.Game.Player.NetworkInput;
using Core.Scripts.Game.Player.PlayerEffects;
using Core.Scripts.Game.Player.PlayerEffects.SimpleEffects;
using Fusion;
using Fusion.Addons.SimpleKCC;
using Sandbox.Project.Scripts.Infrastructure.ModelData.MovementEffects;
using Sandbox.Project.Scripts.Player.PlayersSkins;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game.Player
{
    public enum SensitivityType
    {
        Ox,
        Oy
    }

    public abstract class PlayerData : NetworkBehaviour, IBeforeTick
    {
        [Title("Data", "", TitleAlignments.Right)]
        public TMP_Text nickNameText;
        
        [SerializeField] protected SimpleKCC kcc;
        [SerializeField] protected PlayerInput input;
        
        // [SerializeField] protected PlayerDeathEffect playerDeathEffect;
        [SerializeField] protected PlayerVisualData playerVisualData;
        [SerializeField] protected PlayerBaseMovement playerBaseMovement;
        [SerializeField] private SkinnedMeshRenderer[] skinnedMeshRenderers;
        [SerializeField] private Material playerMaterial;
        
        protected SkinnedMeshRenderer activeSkinnedMeshRenderer;
        protected CancellationTokenSource TokenSource;
        protected Camera MainCamera;
        // protected IVisibilityController SkinVisibilityController;
        
        #region SERVICES
        
        protected ICinemachineService Cinemachine;
        protected IPlayerMovementEffectsListener PlayerMovementEffectsListener;
        protected INickNameFadeEffect FadeEffect;
        protected INetworkPool NetworkPool;

        #endregion SERVICES
        
        [Inject]
        public void Constructor(
            ICinemachineService cinemachine,
            IPlayerMovementEffectsListener playerMovementEffectsListener,
            INickNameFadeEffect nickNameFadeEffect,
            INetworkPool networkPool)
        {
            MainCamera = Camera.main;
            
            NetworkPool = networkPool;
            FadeEffect = nickNameFadeEffect;
            Cinemachine = cinemachine;
            PlayerMovementEffectsListener = playerMovementEffectsListener;
            
            FadeEffect.Initialization(MainCamera);
            
            // SkinVisibilityController = new RendererVisibilityController(playerRenderers);
            // SkinService = new SkinService(SaveLoadSystem,
            //         new JsonSkinAssembler(),
            //         new ClassicBaker(new PlayerSkinBodyPartMapping()), 
            //         new RendererSkinApplier(playerRenderers));
        }

        public abstract void BeforeTick();

        public override void Spawned()
        {
            base.Spawned();
            TokenSource = new CancellationTokenSource();
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            FadeEffect.Dispose();
        }
    }
}
using System.Threading;
using Core.Scripts.Game.Infrastructure.Services.CinemachineService;
using Core.Scripts.Game.Player.Inventory;
using Core.Scripts.Game.Player.Locomotion;
using Core.Scripts.Game.Player.NetworkInput;
using Core.Scripts.Game.Player.PlayerEffects.SimpleEffects;
using Core.Scripts.Game.Player.VisualData;
using Fusion;
using Fusion.Addons.SimpleKCC;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game.Player
{
    public abstract class PlayerData : NetworkBehaviour
    {
        [Title("Data", "", TitleAlignments.Right)]
        public TMP_Text nickNameText;
        
        [SerializeField] protected SimpleKCC kcc;
        [SerializeField] protected PlayerInput input;

        [SerializeField] protected PlayerBaseMovement playerBaseMovement;
        [SerializeField] protected Material playerMaterial;
        [SerializeField] protected PlayerVisual playerVisualData;

        protected CancellationTokenSource TokenSource;
        protected Camera MainCamera;
        
        #region SERVICES
        
        protected ICinemachine Cinemachine;
        protected INickNameFadeEffect FadeEffect;
        protected IPlayerInventory Inventory;

        #endregion SERVICES
        
        [Inject]
        public void Constructor(
            IPlayerInventory inventory,
            ICinemachine cinemachine,
            INickNameFadeEffect nickNameFadeEffect)
        {
            Inventory = inventory;
            MainCamera = Camera.main;
            
            FadeEffect = nickNameFadeEffect;
            Cinemachine = cinemachine;
            
            FadeEffect.Initialization(MainCamera);
        }
        
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
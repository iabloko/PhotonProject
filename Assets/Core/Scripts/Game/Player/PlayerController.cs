using Core.Scripts.Game.Infrastructure.RequiresInjection;
using Core.Scripts.Game.Infrastructure.Services.CinemachineService;
using Core.Scripts.Game.Player.Locomotion;
using Core.Scripts.Game.Player.NetworkInput;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Scripts.Game.Player
{
    public sealed class PlayerController : PlayerInteractor, IAfterSpawned, IBeforeTick, IRequiresInjection
    {
        [Title("Controller", subtitle: "", TitleAlignments.Right), SerializeField]
        private NetworkObject networkObject;
        [SerializeField] private Transform playerModel;
        
        public bool RequiresInjection { get; set; } = true;

        // private int _layerPlayer;
        // private int _layerIgnoreRendering;
        private const string PLAYER_LAYER = "Player";
        private const string IGNORE_RENDERING_LAYER = "IgnoreRendering";

        public override void Spawned()
        {
            base.Spawned();

            if (Object.HasStateAuthority)
            {
                // _layerPlayer = LayerMask.NameToLayer(PLAYER_LAYER);
                // _layerIgnoreRendering = LayerMask.NameToLayer(IGNORE_RENDERING_LAYER);

                SubscribeOnEvents();

                ChangeNetworkSkinIndex();
                ChangeNetworkPlayerNickName();

                SetUpNetworkLocalPlayer();
            }
            else
            {
                FadeEffect.RegisterNickName(nickNameText);
            }
        }

        void IAfterSpawned.AfterSpawned()
        {
            SetNewSkin();
            SetUpLocalPlayerNickName();
            ChangePlayerNicknameVisibility(!Object.HasStateAuthority);
        }
        
        void IBeforeTick.BeforeTick()
        {
            if (!Object.HasStateAuthority) return;

            InputModelData curr = input.CurrentInput;
            InputModelData prev = input.PreviousInput;

            InteractWithObjects(curr, prev);
        }
        
        public override void FixedUpdateNetwork()
        {
            if (!Object.HasStateAuthority) return;
            base.FixedUpdateNetwork();
            FadeEffect.Update();
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            if (Object.HasStateAuthority)
            {
                base.Despawned(runner, hasState);
                TokenSource.Cancel();
                TokenSource.Dispose();

                CancelInvoke();
                UnSubscribeOnEvent();
            }
            else
            {
                FadeEffect.UnregisterNickName(nickNameText);
            }
        }

        private void ChangeNetworkPlayerNickName()
        {
            var networkString = new NetworkString<_16>();
            networkString.Set(PLAYER_LAYER);

            PlayerNickName = networkString;
            transform.name = PLAYER_LAYER;
        }

        private void ChangeNetworkSkinIndex() => SkinIndex = Random.Range(0, playerVisualData.skins.Length - 1);

        private void PrepareToKccTeleportation(Vector3 p, Quaternion r)
        {
            PlayerTeleportationData data = new()
            {
                endPosition = p,
                endRotation = r,
            };

            playerBaseMovement.SaveDataForKccTeleportation(data);
        }

        private void SetUpNetworkLocalPlayer()
        {
            Runner.SetPlayerObject(Runner.LocalPlayer, networkObject);
            PlayerID = Runner.LocalPlayer.PlayerId;
        }

        #region ANALYTICS

        private void SendAnalyticEventOnDead()
        {
            // AnalyticAPI.SendEvents(AnalyticType.All,
            //     new IEventData[]
            //     {
            //         new EventDataModelWithProperties(AmplitudeEventsConstant.Game_PlayerDead)
            //     });
        }

        private void SendAnalyticEventOnCheckPoint(int achievedCheckPoints)
        {
            // AmplitudeEvents.Client_Games_Start_CheckPoint_Properties properties = new(achievedCheckPoints);
            //
            // AnalyticAPI.SendEvents(AnalyticType.All,
            //     new IEventData[]
            //     {
            //         new AmplitudeEvents.Client_Games_Start_CheckPoint_Event(properties)
            //     });
        }

        #endregion ANALYTICS

        #region MOVEMENT EFFECTS

        private void SubscribeOnEvents()
        {
            // GameProgress.OnPlayerFinishedGame += PlayerFinished;
            Cinemachine.OnStateChange += CameraStateChanged;
        }

        private void UnSubscribeOnEvent()
        {
            // GameProgress.OnPlayerFinishedGame -= PlayerFinished;
            Cinemachine.OnStateChange -= CameraStateChanged;
        }

        private void CameraStateChanged(CinemachineState state)
        {
            // ApplyChangedVisibility(state == CinemachineState.NormalFPS ? _layerIgnoreRendering : _layerPlayer);
        }

        #endregion
    }
}
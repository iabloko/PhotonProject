using Core.Scripts.Game.Infrastructure.RequiresInjection;
using Core.Scripts.Game.Infrastructure.Services.CinemachineService;
using Core.Scripts.Game.Player.Locomotion;
using Core.Scripts.Game.Player.NetworkInput;
using Core.Scripts.Game.Player.VisualData;
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
        
        private const string PLAYER_LAYER = "Player";

        public override void Spawned()
        {
            base.Spawned();

            if (Object.HasStateAuthority)
            {
                SubscribeOnEvents();

                ChangeNetworkPlayerVisualData();
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

        private void ChangeNetworkPlayerVisualData()
        {
            int hairId = Random.Range(0, playerVisualData.hair.Length);
            int headId = Random.Range(0, playerVisualData.heads.Length);
            int eyeId = Random.Range(0, playerVisualData.eyes.Length);
            int mouthId = Random.Range(0, playerVisualData.mouth.Length);
            int bodyId = Random.Range(0, playerVisualData.bodies.Length);
            
            VisualNetwork = new PlayerVisualNetwork(hairId, headId, eyeId, mouthId, bodyId);
        }

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
            Cinemachine.OnStateChange += CameraStateChanged;
        }

        private void UnSubscribeOnEvent()
        {
            Cinemachine.OnStateChange -= CameraStateChanged;
        }

        private void CameraStateChanged(CinemachineState state)
        {
            // ApplyChangedVisibility(state == CinemachineState.NormalFPS ? _layerIgnoreRendering : _layerPlayer);
        }

        #endregion
    }
}
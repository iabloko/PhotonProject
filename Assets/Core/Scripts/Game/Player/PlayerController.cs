using Core.Scripts.Game.Infrastructure.RequiresInjection;
using Core.Scripts.Game.Infrastructure.Services.Cinemachine;
using Core.Scripts.Game.Player.Locomotion;
using Cysharp.Threading.Tasks;
using Fusion;
using Sandbox.Project.Scripts.Infrastructure.ModelData.InteractionObjects;
using Sandbox.Project.Scripts.Infrastructure.ModelData.MovementEffects;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Scripts.Game.Player
{
    public sealed class PlayerController : PlayerInteractor, IAfterSpawned, IRequiresInjection
    {
        [Title("Controller", subtitle: "", TitleAlignments.Right), SerializeField]
        private NetworkObject networkObject;
        // [SerializeField] public PlayerBubblePoint bubblePoint;

        [SerializeField] private Transform playerModel;
        
        public bool RequiresInjection { get; set; } = true;

        private int _layerPlayer;
        private int _layerIgnoreRendering;

        public override void Spawned()
        {
            base.Spawned();

            if (Object.HasStateAuthority)
            {
                _layerPlayer = LayerMask.NameToLayer("Player");
                _layerIgnoreRendering = LayerMask.NameToLayer("IgnoreRendering");

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

        public void AfterSpawned()
        {
            SetNewSkin();
            SetUpLocalPlayerNickName();

            if (Object.HasStateAuthority)
            {
                //TODO FIX
                // PrepareToKccTeleportation(
                //     GameManager.LocalInstance.playerCurrentPosition,
                //     GameManager.LocalInstance.playerCurrentRotation);

                PrepareToKccTeleportation(Vector3.up, Quaternion.identity);
                ChangePlayerNicknameVisibility(false);
            }
            else
            {
                ChangePlayerNicknameVisibility(true);
            }
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

        public void PlayerRestart(Vector3 startPosition, Quaternion playerStartRotation)
        {
            playerBaseMovement.PlayerRestart();

            PrepareToKccTeleportation(startPosition, playerStartRotation);
            SetEnablePlayerModel(true);
        }

        private void ChangeNetworkPlayerNickName()
        {
            var networkString = new NetworkString<_16>();
            networkString.Set("Player");

            PlayerNickName = networkString;
            transform.name = "Player";
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

            // PlayerInfo.UpdateUserIdInfo(PlayerID);
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
            PlayerMovementEffectsListener.StartEvent += StartMovementEvent;
        }

        private void UnSubscribeOnEvent()
        {
            // GameProgress.OnPlayerFinishedGame -= PlayerFinished;
            Cinemachine.OnStateChange -= CameraStateChanged;
            PlayerMovementEffectsListener.StartEvent -= StartMovementEvent;
        }

        private void CameraStateChanged(CinemachineState state)
        {
            ApplyChangedVisibility(state == CinemachineState.NormalFPS ? _layerIgnoreRendering : _layerPlayer);
        }

        private void StartMovementEvent(MovementEffectData movementData)
        {
            if (!Object.HasStateAuthority) return;

            switch (movementData.Type)
            {
                case InteractionObjectType.CHECK_POINT:
                    StartCheckPointLogic(movementData);
                    break;
                case InteractionObjectType.LAVA_PLANE:
                case InteractionObjectType.INVISIBLE_LAVA_PLANE:
                case InteractionObjectType.INVISIBLE_LAVA_LANE:
                case InteractionObjectType.DEATH_PLANE:
                    BeforeDeathEffects(movementData);
                    break;
                case InteractionObjectType.PRIMARY_PORTAL:
                case InteractionObjectType.SECOND_PORTAL:
                    EnterPortalLogic();
                    break;
                case InteractionObjectType.EXIT_PORTAL:
                    ExitPortalLogic(movementData);
                    break;
                case InteractionObjectType.NONE:
                case InteractionObjectType.INVISIBLE_WALL:
                case InteractionObjectType.FINISH:
                case InteractionObjectType.JUMP_PAD:
                case InteractionObjectType.BALL:
                case InteractionObjectType.CONTROLLED_CUBE:
                case InteractionObjectType.BANANA:
                case InteractionObjectType.BUBBLE_GUM:
                case InteractionObjectType.SPEED_BOOSTER_PLANE:
                    break;
                default:
                    Debug.LogWarning(
                        $"Start Movement Event call default: {movementData.Type} => {movementData.Transform.name}");
                    break;
            }

            playerBaseMovement.StartMovementEvent(movementData);
        }

        private void EnterPortalLogic()
        {
            // PlayerInfo.PlayerInStatus.ChangePlayerEnterPortalStatus(true);
            SetEnablePlayerModel(false);
            Cinemachine.ChangeCinemachineState(CinemachineState.Teleportation);
        }

        private void ExitPortalLogic(MovementEffectData data)
        {
            SetEnablePlayerModel(true);

            if (data is PortalMovementData portalData)
            {
                Vector3 dirToA = (transform.position - portalData.EndPosition).normalized;
                Quaternion finalRotation = Quaternion.LookRotation(-dirToA);
                PrepareToKccTeleportation(portalData.EndPosition, finalRotation);
            }

            Cinemachine.ChangeCinemachineState(CinemachineState.Normal3Rd);
        }

        private void StartCheckPointLogic(MovementEffectData data)
        {
            Transform colliderTransform = data.Transform;

            //TODO FIX
            // Vector3 v1 = GameManager.LocalInstance.playerCurrentPosition;
            Vector3 v1 = Vector3.zero;
            Vector3 v2 = colliderTransform.position;

            float oy = colliderTransform.rotation.eulerAngles.y;

            // if (Vector3.Distance(v1, v2) > .1f)
            // {
            //     AchievedCheckPoints++;
            //     SendAnalyticEventOnCheckPoint(AchievedCheckPoints);
            // }

            //TODO FIX
            // GameManager.LocalInstance.playerCurrentPosition = v2;
            // GameManager.LocalInstance.playerCurrentRotation = Quaternion.Euler(0, oy, 0);
        }

        private void BeforeDeathEffects(MovementEffectData movementData)
        {
            // ProjectSettings.ChangeGamePauseStatus(true);
            // PlayerInfo.PlayerInStatus.ChangePlayerDeathStatus(true);
            Cinemachine.ChangeCinemachineState(CinemachineState.Teleportation);

            SendAnalyticEventOnDead();

            bool isLine = movementData.Type.Equals(InteractionObjectType.INVISIBLE_LAVA_LANE);
            bool isDeathPlane = movementData.Type.Equals(InteractionObjectType.DEATH_PLANE);

            Transform hitTransform = movementData.Transform;

            Vector3 fxPosition = isDeathPlane ? (transform.position + Vector3.down) : hitTransform.position;
            Vector3 fxRotation = isDeathPlane ? Quaternion.identity.eulerAngles : hitTransform.rotation.eulerAngles;
            Vector3 fxScale = isDeathPlane ? Vector3.one : hitTransform.lossyScale;

            RPC_PlayerDeathLogic(fxPosition, fxRotation, fxScale, isLine);

            Death().Forget();
        }

        private async UniTask Death()
        {
            await UniTask.Delay(750);

            //TODO FIX
            // PrepareToKccTeleportation(
            //     GameManager.LocalInstance.playerCurrentPosition,
            //     GameManager.LocalInstance.playerCurrentRotation);

            PrepareToKccTeleportation(Vector3.up, Quaternion.identity);

            await UniTask.DelayFrame(10);

            AfterDeathEffects();
        }

        private void AfterDeathEffects()
        {
            // PlayerInfo.PlayerInStatus.ChangePlayerDeathStatus(false);

            //TODO FIX
            // if (!Game.GameMenuUIStateMachine.IsOpened)
            // {
            //     ProjectSettings.ChangeGamePauseStatus(false);
            // }

            Cinemachine.ChangeCinemachineState(CinemachineState.Normal3Rd);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game.Infrastructure.Services.Cinemachine
{
    public sealed class CinemachineService : ICinemachineService
    {
        private const int ACTIVE_PRIORITY = 20;
        private const int INACTIVE_PRIORITY = 10;

        private readonly IAssetProvider _provider;
        private CancellationTokenSource _token;

        private Transform _root;
        private bool _initialized;

        private readonly Dictionary<CinemachineState, CinemachineVirtualRig> _virtualRigs = new(4);

        // private CinemachineVirtualCamera _vcam3Rd;
        // private CinemachineVirtualCamera _vcamFps;
        // private CinemachineVirtualCamera _vcamPrev;
        // private CinemachineVirtualCamera _vcamTps;
        private Transform _player;

        public int PreviewDirection { get; private set; } = 1;
        public CinemachineState CurrentState { get; private set; } = CinemachineState.Preview;
        public Action<CinemachineState> OnStateChange { get; set; }

        public float CurrentCameraDistance { get; private set; }

        [Inject]
        public CinemachineService(IAssetProvider provider)
        {
            _provider = provider;
        }

        public void ChangeVCamFieldOfView(int newValue)
        {
            // foreach (var rig in _virtualRigs)
            // {
            //     rig.Value.VCam.m_Lens.FarClipPlane = newValue;
            // }
        }

        void ICinemachineService.Register(Transform player, Transform previewRotation, Vector2 pitchYawDeg)
        {
            // _token = new CancellationTokenSource();
            //
            // if (_initialized) return;
            // _player = player;
            //
            // _root = new GameObject("Virtual_Cameras").transform;
            //
            // _vcam3Rd = CreateVcam(AssetPaths.PLAYER_VIRTUAL_CAMERA_3_RD, "VCam_3rd");
            // _vcamFps = CreateVcam(AssetPaths.PLAYER_VIRTUAL_CAMERA_FPS, "VCam_FPS");
            // _vcamPrev = CreateVcam(AssetPaths.PLAYER_VIRTUAL_CAMERA_PREVIEW, "VCam_Preview");
            // _vcamTps = CreateVcam(AssetPaths.PLAYER_VIRTUAL_CAMERA_TELEPORTATION, "VCam_Teleport");
            //
            // HookOnLive(_vcam3Rd, CinemachineState.Normal3Rd);
            // HookOnLive(_vcamFps, CinemachineState.NormalFPS);
            // HookOnLive(_vcamPrev, CinemachineState.Preview);
            // HookOnLive(_vcamTps, CinemachineState.Teleportation);
            //
            // _vcam3Rd.Follow = _vcam3Rd.LookAt = player;
            // _vcamFps.Follow = _vcamFps.LookAt = player;
            // _vcamTps.Follow = _vcamTps.LookAt = player;
            //
            // _vcamPrev.Follow = _vcamPrev.LookAt = previewRotation;
            //
            // _virtualRigs[CinemachineState.Normal3Rd] = new CinemachineVirtualRig(_vcam3Rd);
            // _virtualRigs[CinemachineState.NormalFPS] = new CinemachineVirtualRig(_vcamFps);
            // _virtualRigs[CinemachineState.Preview] = new CinemachineVirtualRig(_vcamPrev);
            // _virtualRigs[CinemachineState.Teleportation] = new CinemachineVirtualRig(_vcamTps);
            //
            // ConfigurePov(_virtualRigs[CinemachineState.Normal3Rd].Pov, pitchYawDeg);
            // ConfigurePov(_virtualRigs[CinemachineState.NormalFPS].Pov, pitchYawDeg);
            //
            // CinemachineFramingTransposer transposer3Rd = _virtualRigs[CinemachineState.Normal3Rd].Transposer;
            // if (transposer3Rd != null) transposer3Rd.m_CameraDistance = PlayerInfo.CameraDistance;
            //
            // SetAllPriorities(INACTIVE_PRIORITY);
            // _virtualRigs[CinemachineState.Preview].SetPriority(ACTIVE_PRIORITY);
            // CurrentState = CinemachineState.Preview;
            //
            // _initialized = true;
        }

        void ICinemachineService.ChangeCinemachineState(CinemachineState state)
        {
            EnsureInitialized();

            CurrentState = state;

            if (state == CinemachineState.Preview)
                PreviewDirection = UnityEngine.Random.Range(0, 2) == 1 ? 1 : -1;

            SetAllPriorities(INACTIVE_PRIORITY);

            CinemachineVirtualRig currentRig = _virtualRigs[state];
            // currentRig.SetPriority(ACTIVE_PRIORITY);

            CurrentCameraDistance = state switch
            {
                // CinemachineState.Normal3Rd => currentRig.Transposer.m_CameraDistance,
                CinemachineState.NormalFPS => 0,
                _ => CurrentCameraDistance
            };
        }

        void ICinemachineService.UpdateVCam(Vector2 pitchYawDeg)
        {
            if (!_initialized) return;

            // foreach (CinemachineVirtualRig rig in _virtualRigs.Values.Where(rig => rig.Pov != null))
            // {
            //     rig.Pov.m_HorizontalAxis.Value = pitchYawDeg.y;
            //     rig.Pov.m_VerticalAxis.Value = pitchYawDeg.x;
            // }

            // if (float.IsNaN(pitchYawDeg.x) || float.IsNaN(pitchYawDeg.y)) return;
            //
            // foreach (CinemachineVirtualRig rig in _virtualRigs.Values)
            //     DrivePovTowards(rig.Pov, pitchYawDeg);
        }

        // private static void DrivePovTowards(CinemachinePOV pov, Vector2 pitchYawDeg)
        // {
        //     if (!pov) return;
        //
        //     float targetYaw = Normalize180(pitchYawDeg.y);
        //     float targetPitch = Normalize180(pitchYawDeg.x);
        //
        //     float curYaw = pov.m_HorizontalAxis.Value;
        //     float curPitch = pov.m_VerticalAxis.Value;
        //
        //     pov.m_HorizontalAxis.Value = curYaw + Mathf.DeltaAngle(curYaw, targetYaw);
        //     pov.m_VerticalAxis.Value = curPitch + Mathf.DeltaAngle(curPitch, targetPitch);
        //
        //     pov.m_VerticalAxis.Value = Mathf.Clamp(
        //         pov.m_VerticalAxis.Value,
        //         pov.m_VerticalAxis.m_MinValue,
        //         pov.m_VerticalAxis.m_MaxValue
        //     );
        // }

        private static float Normalize180(float angle)
        {
            angle %= 360f;
            if (angle > 180f) angle -= 360f;
            if (angle <= -180f) angle += 360f;
            return angle;
        }

        void ICinemachineService.ChangeVCamVerticalSensitivity(float value)
        {
            // if (_virtualRigs.TryGetValue(CinemachineState.Normal3Rd, out CinemachineVirtualRig rig3Rd) &&
            //     rig3Rd.Pov != null)
            //     rig3Rd.Pov.m_VerticalAxis.m_MaxSpeed = value;
            //
            // if (_virtualRigs.TryGetValue(CinemachineState.NormalFPS, out CinemachineVirtualRig rigFps) &&
            //     rigFps.Pov != null)
            //     rigFps.Pov.m_VerticalAxis.m_MaxSpeed = value;
        }

        void ICinemachineService.ChangeVCamDistance(float distance)
        {
            if (!_initialized) return;

            // if (!_virtualRigs.TryGetValue(CinemachineState.Normal3Rd, out CinemachineVirtualRig rig3Rd) ||
            //     rig3Rd.Transposer == null) return;
            //
            // rig3Rd.Transposer.m_CameraDistance = distance;
            CurrentCameraDistance = distance;
            // PlayerInfo.UpdateCameraDistance(distance);
        }

        void ICinemachineService.Dispose()
        {
            _token.Cancel();
            _token.Dispose();

            // _vcam3Rd.m_Transitions.m_OnCameraLive.RemoveAllListeners();
            // _vcamFps.m_Transitions.m_OnCameraLive.RemoveAllListeners();
            // _vcamPrev.m_Transitions.m_OnCameraLive.RemoveAllListeners();
            // _vcamTps.m_Transitions.m_OnCameraLive.RemoveAllListeners();

            if (_root != null)
            {
                UnityEngine.Object.Destroy(_root.gameObject);
                _root = null;
            }

            _virtualRigs.Clear();
            _initialized = false;
        }

        // private void HookOnLive(CinemachineVirtualCamera vcam, CinemachineState stateToReport)
        // {
        //     vcam.m_Transitions.m_OnCameraLive.AddListener((incoming, _) =>
        //     {
        //         if (ReferenceEquals(incoming, vcam))
        //             OnStateChange?.Invoke(stateToReport);
        //     });
        // }
        //
        // private CinemachineVirtualCamera CreateVcam(string assetPath, string name)
        // {
        //     CinemachineVirtualCamera vcam =
        //         _projectFactory.Instantiate<CinemachineVirtualCamera>(assetPath, false, _root);
        //     vcam.transform.name = name;
        //     vcam.Priority = INACTIVE_PRIORITY;
        //     return vcam;
        // }
        //
        // private static void ConfigurePov(CinemachinePOV pov, Vector2 pitchYawDeg)
        // {
        //     if (pov == null) return;
        //
        //     pov.m_HorizontalAxis.m_InputAxisName = string.Empty;
        //     pov.m_VerticalAxis.m_InputAxisName = string.Empty;
        //     pov.m_HorizontalAxis.m_InputAxisValue = 0f;
        //     pov.m_VerticalAxis.m_InputAxisValue = 0f;
        //
        //     pov.m_HorizontalAxis.m_AccelTime = 0f;
        //     pov.m_HorizontalAxis.m_DecelTime = 0f;
        //     pov.m_VerticalAxis.m_AccelTime = 0f;
        //     pov.m_VerticalAxis.m_DecelTime = 0f;
        //
        //     pov.m_HorizontalAxis.Value = pitchYawDeg.y;
        //     pov.m_VerticalAxis.Value = pitchYawDeg.x;
        // }

        private void SetAllPriorities(int value)
        {
            // foreach (CinemachineVirtualRig rig in _virtualRigs.Values)
            //     rig.SetPriority(value);
        }

        private void EnsureInitialized()
        {
            if (_initialized) return;
            throw new InvalidOperationException("CinemachineService is not initialized. Call Register() first.");
        }
    }
}
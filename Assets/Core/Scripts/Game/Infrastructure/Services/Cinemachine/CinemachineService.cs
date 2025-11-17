using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Unity.Cinemachine;
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

        private CinemachineCamera _vcam3Rd;
        private CinemachineCamera _vcamFps;
        private CinemachineCamera _vcamPrev;
        private CinemachineCamera _vcamTps;

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

        void ICinemachineService.ChangeCamFarClipPlane(int newFarClip)
        {
            foreach (var rig in _virtualRigs)
            {
                LensSettings lens = rig.Value.VCam.Lens;
                lens.FarClipPlane = newFarClip;
                rig.Value.VCam.Lens = lens;
            }
        }

        void ICinemachineService.Register(Transform player, Transform previewRotation, Vector2 pitchYawDeg)
        {
            _token = new CancellationTokenSource();

            if (_initialized) return;

            _player = player;
            _root = new GameObject("Virtual_Cameras").transform;

            _vcam3Rd = CreateVcam(AssetPaths.PLAYER_VIRTUAL_CAMERA_3_RD, "VCam_3rd");
            _vcamFps = CreateVcam(AssetPaths.PLAYER_VIRTUAL_CAMERA_FPS, "VCam_FPS");
            _vcamPrev = CreateVcam(AssetPaths.PLAYER_VIRTUAL_CAMERA_PREVIEW, "VCam_Preview");
            _vcamTps = CreateVcam(AssetPaths.PLAYER_VIRTUAL_CAMERA_TELEPORTATION, "VCam_Teleport");

            _vcam3Rd.Follow = _vcam3Rd.LookAt = player;
            _vcamFps.Follow = _vcamFps.LookAt = player;
            _vcamTps.Follow = _vcamTps.LookAt = player;
            _vcamPrev.Follow = _vcamPrev.LookAt = previewRotation;

            _virtualRigs[CinemachineState.Normal3Rd] = new CinemachineVirtualRig(_vcam3Rd);
            _virtualRigs[CinemachineState.NormalFPS] = new CinemachineVirtualRig(_vcamFps);
            _virtualRigs[CinemachineState.Preview] = new CinemachineVirtualRig(_vcamPrev);
            _virtualRigs[CinemachineState.Teleportation] = new CinemachineVirtualRig(_vcamTps);

            ConfigurePov(_virtualRigs[CinemachineState.Normal3Rd].Pov, pitchYawDeg);
            ConfigurePov(_virtualRigs[CinemachineState.NormalFPS].Pov, pitchYawDeg);

            SetAllPriorities(INACTIVE_PRIORITY);
            _virtualRigs[CinemachineState.Preview].SetPriority(ACTIVE_PRIORITY);
            CurrentState = CinemachineState.Preview;

            SubscribeCameraActivationEvents();

            _initialized = true;
        }

        void ICinemachineService.ChangeCinemachineState(CinemachineState state)
        {
            EnsureInitialized();

            CurrentState = state;

            if (state == CinemachineState.Preview)
                PreviewDirection = UnityEngine.Random.Range(0, 2) == 1 ? 1 : -1;

            SetAllPriorities(INACTIVE_PRIORITY);

            CinemachineVirtualRig currentRig = _virtualRigs[state];
            currentRig.SetPriority(ACTIVE_PRIORITY);

            CurrentCameraDistance = state switch
            {
                CinemachineState.Normal3Rd => currentRig.Transposer.CameraDistance,
                CinemachineState.NormalFPS => 0,
                _ => CurrentCameraDistance
            };
        }

        void ICinemachineService.UpdateVCam(Vector2 pitchYawDeg)
        {
            if (!_initialized) return;
            
            foreach (CinemachineVirtualRig rig in _virtualRigs.Values.Where(r => r.Pov != null))
            {
                rig.Pov.PanAxis.Value = pitchYawDeg.y;
                rig.Pov.TiltAxis.Value = pitchYawDeg.x;
            }

            if (float.IsNaN(pitchYawDeg.x) || float.IsNaN(pitchYawDeg.y)) return;

            foreach (CinemachineVirtualRig rig in _virtualRigs.Values)
                DrivePovTowards(rig.Pov, pitchYawDeg);
        }
        

        private static void DrivePovTowards(CinemachinePanTilt pov, Vector2 pitchYawDeg)
        {
            if (!pov) return;

            float targetYaw = Normalize180(pitchYawDeg.y);
            float targetPitch = Normalize180(pitchYawDeg.x);

            float curYaw = pov.PanAxis.Value;
            float curPitch = pov.TiltAxis.Value;

            pov.PanAxis.Value = curYaw + Mathf.DeltaAngle(curYaw, targetYaw);
            pov.TiltAxis.Value = curPitch + Mathf.DeltaAngle(curPitch, targetPitch);
            
            Vector2 range = pov.TiltAxis.Range;
            pov.TiltAxis.Value = Mathf.Clamp(pov.TiltAxis.Value, range.x, range.y);
        }

        private static float Normalize180(float angle)
        {
            angle %= 360f;
            if (angle > 180f) angle -= 360f;
            if (angle <= -180f) angle += 360f;
            return angle;
        }
        
        void ICinemachineService.ChangeVCamDistance(float distance)
        {
            if (!_initialized) return;

            if (!_virtualRigs.TryGetValue(CinemachineState.Normal3Rd, out CinemachineVirtualRig rig3Rd) ||
                rig3Rd.Transposer == null) return;

            rig3Rd.Transposer.CameraDistance = distance;
            CurrentCameraDistance = distance;
        }
        
        void ICinemachineService.Dispose()
        {
            if (_token != null)
            {
                _token.Cancel();
                _token.Dispose();
                _token = null;
            }

            UnsubscribeCameraActivationEvents();

            if (_root != null)
            {
                UnityEngine.Object.Destroy(_root.gameObject);
                _root = null;
            }

            _vcam3Rd = null;
            _vcamFps = null;
            _vcamPrev = null;
            _vcamTps = null;

            _virtualRigs.Clear();
            _initialized = false;
        }
        
        private void SubscribeCameraActivationEvents()
        {
            CinemachineCore.CameraActivatedEvent.AddListener(OnCameraActivated);
        }

        private void UnsubscribeCameraActivationEvents()
        {
            CinemachineCore.CameraActivatedEvent.RemoveListener(OnCameraActivated);
        }

        private void OnCameraActivated(ICinemachineCamera.ActivationEventParams args)
        {
            if (!_initialized) return;

            if (ReferenceEquals(args.IncomingCamera, _vcam3Rd))
            {
                OnStateChange?.Invoke(CinemachineState.Normal3Rd);
            }
            else if (ReferenceEquals(args.IncomingCamera, _vcamFps))
            {
                OnStateChange?.Invoke(CinemachineState.NormalFPS);
            }
            else if (ReferenceEquals(args.IncomingCamera, _vcamPrev))
            {
                OnStateChange?.Invoke(CinemachineState.Preview);
            }
            else if (ReferenceEquals(args.IncomingCamera, _vcamTps))
            {
                OnStateChange?.Invoke(CinemachineState.Teleportation);
            }
        }
        
        private CinemachineCamera CreateVcam(string assetPath, string name)
        {
            CinemachineCamera vcam = _provider.InstantiateObject<CinemachineCamera>(assetPath, _root);
            vcam.transform.name = name;
            vcam.Priority = INACTIVE_PRIORITY;
            return vcam;
        }

        private static void ConfigurePov(CinemachinePanTilt pov, Vector2 pitchYawDeg)
        {
            if (pov == null) return;
            
            pov.PanAxis.Value = pitchYawDeg.y;
            pov.TiltAxis.Value = pitchYawDeg.x;
        }

        private void SetAllPriorities(int value)
        {
            foreach (CinemachineVirtualRig rig in _virtualRigs.Values)
                rig.SetPriority(value);
        }

        private void EnsureInitialized()
        {
            if (_initialized) return;
            throw new InvalidOperationException("CinemachineService is not initialized. Call Register() first.");
        }
    }
}

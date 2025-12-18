using Core.Scripts.Game.Infrastructure.Services.CinemachineService;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using Core.Scripts.Game.PlayerLogic.ContextLogic;
using UnityEngine;

namespace Core.Scripts.Game.PlayerLogic.Movement
{
    public sealed class Rotation
    {
        private readonly PlayerContext _ctx;
        private readonly ICinemachine _cinemachine;
        private readonly Transform _previewRotation;
        private readonly float _rotationSpeed;

        private float _displayPitch;
        private float _desiredYaw;

        private Camera _mainCamera;
        private Vector2 PitchRotation => _ctx.Kcc.GetLookRotation(pitch: true, yaw: true);
        
        private bool _isFpsMode = false;
        private readonly IProjectSettings _projectSettings;

        public Rotation(PlayerContext ctx,
            ICinemachine cinemachine,
            IProjectSettings projectSettings,
            Transform previewRotation,
            float rotationSpeed)
        {
            _ctx = ctx;
            _projectSettings = projectSettings;
            _cinemachine = cinemachine;
            _previewRotation = previewRotation;
            _rotationSpeed = rotationSpeed;
        }

        public void AfterSpawned()
        {
            _cinemachine.Register(_ctx.Kcc.transform, _previewRotation, PitchRotation);
            _cinemachine.ChangeCinemachineState(CinemachineState.Normal3Rd);

            SetNormalCamera();
            UpdatePreviewRotation();

            _isFpsMode = false;
        }

        public void FixedUpdateNetwork()
        {
            if (_projectSettings.IsGamePaused) return;
            
            Vector2 lookDelta = _ctx.Input.CurrentInput.LookRotationDelta;
            _ctx.Kcc.AddLookRotation(lookDelta);
        }

        public void LateUpdate()
        {
            if (_projectSettings.IsGamePaused) return;
            
            FirstPersonSwitcher();

            if (_cinemachine.CurrentState == CinemachineState.Normal3Rd &&
                (Mathf.Abs(_ctx.Input.ScrollWheelRaw) > Mathf.Epsilon))
                _cinemachine.ChangeVCamDistance(_ctx.Input.ScrollWheel);

            _cinemachine.UpdateVCam(PitchRotation);

            RotateCameraAfkMode();
        }

        private void SetNormalCamera() => _mainCamera = Camera.main;

        private void UpdatePreviewRotation()
        {
            Vector3 camEuler = _mainCamera.transform.rotation.eulerAngles;
            float cachedY = camEuler.y;
            _previewRotation.rotation = Quaternion.Euler(0f, cachedY, 0f);
        }

        private void FirstPersonSwitcher()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                _isFpsMode = !_isFpsMode;
                CinemachineState state = _isFpsMode ? CinemachineState.NormalFPS : CinemachineState.Normal3Rd;
                _cinemachine.ChangeCinemachineState(state);
            }
        }

        private void RotateCameraAfkMode()
        {
            if (_cinemachine.CurrentState.Equals(CinemachineState.Preview))
            {
                _previewRotation.Rotate(0, _cinemachine.PreviewDirection * Time.deltaTime * _rotationSpeed, 0,
                    Space.World);
            }
            else if (_cinemachine.CurrentState.Equals(CinemachineState.Normal3Rd))
            {
                UpdatePreviewRotation();
            }
        }
    }
}

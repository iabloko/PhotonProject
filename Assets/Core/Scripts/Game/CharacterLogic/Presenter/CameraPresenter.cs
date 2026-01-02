using Core.Scripts.Game.Infrastructure.Services.CinemachineService;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using UnityEngine;

namespace Core.Scripts.Game.CharacterLogic.Presenter
{
    public sealed class CameraPresenter
    {
        private readonly ICharacterMotor _motor;
        private readonly ICharacterInput _input;
        private readonly ICinemachine _cinemachine;
        private readonly IProjectSettings _projectSettings;
        private readonly Transform _preview;
        private readonly float _rotationSpeed;

        private Camera _cam;
        private bool _isFps;

        public CameraPresenter(ICharacterMotor motor, ICharacterInput input, ICinemachine cinemachine,
            IProjectSettings projectSettings, Transform preview, float rotationSpeed)
        {
            _motor = motor;
            _input = input;
            _cinemachine = cinemachine;
            _projectSettings = projectSettings;
            _preview = preview;
            _rotationSpeed = rotationSpeed;
        }

        public void AfterSpawned()
        {
            _cam = Camera.main;
            _cinemachine.Register(_motor.Transform, _preview, _motor.GetLookRotation(pitch: true, yaw: true));
            _cinemachine.ChangeCinemachineState(CinemachineState.Normal3Rd);
            SyncPreviewToCamera();
            _isFps = false;
        }

        public void LateUpdate()
        {
            if (_projectSettings.IsGamePaused) return;

            if (_input.ToggleFpsPressed)
            {
                _isFps = !_isFps;
                CinemachineState state = _isFps ? CinemachineState.NormalFPS : CinemachineState.Normal3Rd;
                _cinemachine.ChangeCinemachineState(state);
            }

            if (_cinemachine.CurrentState == CinemachineState.Normal3Rd && Mathf.Abs(_input.Scroll) > Mathf.Epsilon)
                _cinemachine.ChangeVCamDistance(_input.Scroll);

            _cinemachine.UpdateVCam(_motor.GetLookRotation(true, true));
            RotatePreviewAfk();
        }

        private void SyncPreviewToCamera()
        {
            if (_cam == null) return;
            float y = _cam.transform.rotation.eulerAngles.y;
            _preview.rotation = Quaternion.Euler(0f, y, 0f);
        }

        private void RotatePreviewAfk()
        {
            if (_cinemachine.CurrentState == CinemachineState.Preview)
            {
                _preview.Rotate(0, _cinemachine.PreviewDirection * Time.deltaTime * _rotationSpeed, 0, Space.World);
            }
            else if (_cinemachine.CurrentState == CinemachineState.Normal3Rd)
            {
                SyncPreviewToCamera();
            }
        }
    }
}
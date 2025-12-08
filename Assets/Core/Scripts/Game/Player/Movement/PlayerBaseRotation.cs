using Core.Scripts.Game.Infrastructure.Services.CinemachineService;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.Player.Movement
{
    public abstract class PlayerBaseRotation : PlayerBaseAnimation
    {
        [Title("Rotation", "Cinemachine", TitleAlignments.Right), SerializeField]
        private Transform previewRotation;
        [SerializeField] private float rotationSpeed = 2;
        
        private float _displayPitch;
        private float _desiredYaw;

        private Camera _mainCamera;
        private Vector2 PitchRotation => kcc.GetLookRotation(pitch: true, yaw: true);
        private bool _isFpsMode = false;

        public override void AfterSpawned()
        {
            if (Object.HasStateAuthority)
            {
                Cinemachine.Register(transform, previewRotation, PitchRotation);
                Cinemachine.ChangeCinemachineState(CinemachineState.Normal3Rd);

                SetNormalCamera();
                UpdatePreviewRotation();

                _isFpsMode = false;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (ProjectSettings.IsGamePaused) return;
            
            Vector2 lookDelta = input.CurrentInput.LookRotationDelta;
            kcc.AddLookRotation(lookDelta);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (Object.HasStateAuthority)
            {
                base.Despawned(runner, hasState);
                Cinemachine.Dispose();
            }
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            
            if (ProjectSettings.IsGamePaused) return;
            
            FirstPersonSwitcher();

            if (Cinemachine.CurrentState == CinemachineState.Normal3Rd &&
                (Mathf.Abs(input.ScrollWheelRaw) > Mathf.Epsilon))
                Cinemachine.ChangeVCamDistance(input.ScrollWheel);

            Cinemachine.UpdateVCam(PitchRotation);

            RotateCameraAfkMode();
        }

        private void FirstPersonSwitcher()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                _isFpsMode = !_isFpsMode;
                CinemachineState state = _isFpsMode ? CinemachineState.NormalFPS : CinemachineState.Normal3Rd;
                Cinemachine.ChangeCinemachineState(state);
            }
        }

        private void RotateCameraAfkMode()
        {
            if (Cinemachine.CurrentState.Equals(CinemachineState.Preview))
            {
                previewRotation.Rotate(0, Cinemachine.PreviewDirection * Time.deltaTime * rotationSpeed, 0,
                    Space.World);
            }
            else if (Cinemachine.CurrentState.Equals(CinemachineState.Normal3Rd))
            {
                UpdatePreviewRotation();
            }
        }

        private void UpdatePreviewRotation()
        {
            Vector3 camEuler = _mainCamera.transform.rotation.eulerAngles;
            float cachedY = camEuler.y;
            previewRotation.rotation = Quaternion.Euler(0f, cachedY, 0f);
        }

        private void SetNormalCamera() => _mainCamera = Camera.main;
    }
}
using Sandbox.Project.Scripts.Helpers.TransformHelpers;
using UnityEngine;

namespace Core.Scripts.Game.Player.PlayerEffects.SimpleEffects
{
    public sealed class CameraShake : IPlayerEffect
    {
        private readonly Transform _cameraOrigin;

        private Vector3 _cameraOriginTarget;
        private Vector3 _defaultCameraOriginTarget;

        private const float CAMERA_INTERPOLATION_DECAY = 16;
        private const float RUN_CAMERA_SHAKE_FREQ = 40f;
        private const float RUN_CAMERA_SHAKE_AMPLITUDE = .1f;

        public void OnPlayerMovement()
        {
            Vector3 cameraShakeAmplitude = CalculateCameraShakeAmplitude();
            _cameraOriginTarget = _defaultCameraOriginTarget + cameraShakeAmplitude;
        }

        public void OnPlayerStop()
        {
            _cameraOriginTarget = _defaultCameraOriginTarget;
        }

        private Vector3 CalculateCameraShakeAmplitude()
        {
            float sinFunction = Mathf.Sin(Time.time * RUN_CAMERA_SHAKE_FREQ);
            Vector3 shakeAmplitude = Vector3.up * RUN_CAMERA_SHAKE_AMPLITUDE;
            return Mathf.Sin(sinFunction) * shakeAmplitude;
        }

        public void OnUpdateCall()
        {
            _cameraOrigin.Decay(_cameraOrigin.localPosition, _cameraOriginTarget, CAMERA_INTERPOLATION_DECAY, Time.deltaTime);
        }
    }
}
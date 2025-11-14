using System;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.Cinemachine
{
    public enum CinemachineState
    {
        Preview,
        Normal3Rd,
        NormalFPS,
        Teleportation,
    }
    
    public interface ICinemachineService
    {
        public int PreviewDirection { get; }
        public CinemachineState CurrentState { get; }
        public Action<CinemachineState> OnStateChange { get; set; }
        
        public float CurrentCameraDistance { get; }
        
        public void ChangeCinemachineState(CinemachineState state);
        public void ChangeVCamVerticalSensitivity(float value);
        public void ChangeVCamDistance(float delta);
        public void ChangeVCamFieldOfView(int newValue);
        
        public void Register(Transform player, Transform previewRotation, Vector2 pitchRotation);
        public void Dispose();
        public void UpdateVCam(Vector2 pitchRotation);
    }
}
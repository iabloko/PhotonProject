using Unity.Cinemachine;

namespace Core.Scripts.Game.Infrastructure.Services.CinemachineService
{
    public sealed class CinemachineVirtualRig
    {
        public readonly CinemachineCamera VCam;
        public readonly CinemachinePanTilt Pov;
        public readonly CinemachinePositionComposer Transposer;
        
        public CinemachineVirtualRig(CinemachineCamera vcam)
        {
            VCam = vcam;
            Pov = vcam ? vcam.GetComponent<CinemachinePanTilt>() : null;
            Transposer = vcam ? vcam.GetComponent<CinemachinePositionComposer>() : null;
        }

        public void SetPriority(int value) => VCam.Priority = value;
    }
}
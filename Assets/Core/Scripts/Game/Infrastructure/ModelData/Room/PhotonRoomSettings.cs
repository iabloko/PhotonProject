using System;
using Sirenix.OdinInspector;
using UnityEngine.Scripting;

namespace Core.Scripts.Game.Infrastructure.ModelData.Room
{
    [BoxGroup("GAME ROOM SETTINGS"), Serializable, Preserve]
    public struct PhotonRoomSettings
    {
        public float walkingSpeed;
        public float runningSpeed;
        public float jumpFactor;
        public float jumpInertia;
        public bool autoBunnyHop;
        public bool shiftMode;
        public bool autoRun;

        [BoxGroup("Local Data")] public float upGravity;
        [BoxGroup("Local Data")] public float downGravity;
        [BoxGroup("Local Data")] public float groundAcceleration;
        [BoxGroup("Local Data")] public float groundDeceleration;
        [BoxGroup("Local Data")] public float airAcceleration;
        [BoxGroup("Local Data")] public float airDeceleration;
        [BoxGroup("Local Data")] public float speedUpBoost;
        [BoxGroup("Local Data")] public float localJumpForce;
        [BoxGroup("Local Data")] public float jumpPadAirDeceleration;
        [BoxGroup("Local Data")] public float jumpPadAirDecelerationMovement;
        
        public static PhotonRoomSettings CreateDefault() => new()
        {
            walkingSpeed = 10f,
            runningSpeed = 20f,
            jumpFactor = 1.5f,
            jumpInertia = 0f,
            autoBunnyHop = false,
            shiftMode = false,
            autoRun = false,

            upGravity = -50f,
            downGravity = -90f,
            groundAcceleration = 55f,
            groundDeceleration = 25f,
            airAcceleration = 25f,
            airDeceleration = 1.3f,
            speedUpBoost = 2f,
            localJumpForce = 13.77f,
            jumpPadAirDeceleration = 0.35f,
            jumpPadAirDecelerationMovement = 1.1f,
        };

        public static readonly PhotonRoomSettings Default = CreateDefault();
    }
}
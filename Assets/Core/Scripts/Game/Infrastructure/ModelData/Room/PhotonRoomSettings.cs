using System;
using Sirenix.OdinInspector;
using UnityEngine.Scripting;

namespace Core.Scripts.Game.Infrastructure.ModelData.Room
{
    [BoxGroup("ROOM SETTINGS"), Serializable, HideLabel, Preserve]
    public struct PhotonRoomSettings
    {
        [Title("Game Data", subtitle: "")]
        public float walkingSpeed;
        public float runningSpeed;
        public float jumpFactor;
        public float jumpInertia;
        public bool shiftMode;
        public bool autoRun;

        [Title("Local Data", subtitle: "")]
        public float upGravity;
        public float downGravity;
        public float groundAcceleration;
        public float groundDeceleration;
        public float airAcceleration;
        public float airDeceleration;
        public float speedUpBoost;
        public float localJumpForce;
        public float jumpPadAirDeceleration;
        public float jumpPadAirDecelerationMovement;

        public static PhotonRoomSettings CreateDefault() => new()
        {
            walkingSpeed = 5f,
            runningSpeed = 10f,
            jumpFactor = 1.5f,
            jumpInertia = 0f,
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

        [Button(ButtonSizes.Medium)]
        public void FillBaseRoomSettings()
        {
            walkingSpeed = 5f;
            runningSpeed = 10f;
            jumpFactor = 1.5f;
            jumpInertia = 0f;
            shiftMode = false;
            autoRun = false;

            upGravity = -50f;
            downGravity = -90f;
            groundAcceleration = 55f;
            groundDeceleration = 25f;
            airAcceleration = 25f;
            airDeceleration = 1.3f;
            speedUpBoost = 2f;
            localJumpForce = 13.77f;
            jumpPadAirDeceleration = 0.35f;
            jumpPadAirDecelerationMovement = 1.1f;
        }
    }
}
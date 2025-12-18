using UnityEngine;

namespace Core.Scripts.Game.CharacterLogic
{
    public interface ICharacterMotor
    {
        Transform Transform { get; }
        Vector3 Position { get; }
        Vector3 RealVelocity { get; }
        bool IsGrounded { get; }

        Quaternion TransformRotation { get; }
        Vector3 TransformDirection { get; }

        void SetMaxGroundAngle(float angle);
        void SetGravity(float gravity);
        void ResetVelocity();
        void Move(Vector3 velocity, float jumpImpulse = 0f);

        void AddLookRotation(Vector2 delta);
        Vector2 GetLookRotation(bool pitch = true, bool yaw = true);

        bool ProjectOnGround(Vector3 vector, out Vector3 projectedVector);
    }
}
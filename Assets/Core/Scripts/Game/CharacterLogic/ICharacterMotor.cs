using UnityEngine;

namespace Core.Scripts.Game.CharacterLogic
{
    public interface ICharacterMotor
    {
        public Transform Transform { get; }
        public Vector3 Position { get; }
        public Vector3 RealVelocity { get; }
        public bool IsGrounded { get; }

        public Quaternion TransformRotation { get; }
        public Vector3 TransformDirection { get; }

        public void SetMaxGroundAngle(float angle);
        public void SetGravity(float gravity);
        public void ResetVelocity();
        public void Move(Vector3 velocity, float jumpImpulse = 0f);
        public void AddLookRotation(Vector2 delta);
        public Vector2 GetLookRotation(bool pitch = true, bool yaw = true);
        public bool ProjectOnGround(Vector3 vector, out Vector3 projectedVector);
        public void SetColliderLayer(string layer);
        public void SetColliderTag(string tag);
    }
}
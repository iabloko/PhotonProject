using Fusion.Addons.SimpleKCC;
using UnityEngine;

namespace Core.Scripts.Game.CharacterLogic.Adapters
{
    public sealed class KccMotorAdapter : ICharacterMotor
    {
        private readonly SimpleKCC _kcc;

        public KccMotorAdapter(SimpleKCC kcc) => _kcc = kcc;

        public Transform Transform => _kcc.transform;
        public Vector3 Position => _kcc.transform.position;
        public Vector3 RealVelocity => _kcc.RealVelocity;
        public bool IsGrounded => _kcc.IsGrounded;

        public Quaternion TransformRotation => _kcc.TransformRotation;
        public Vector3 TransformDirection => _kcc.TransformDirection;

        public void SetMaxGroundAngle(float angle) => _kcc.SetMaxGroundAngle(angle);
        public void SetGravity(float gravity) => _kcc.SetGravity(gravity);
        public void ResetVelocity() => _kcc.ResetVelocity();
        public void Move(Vector3 velocity, float jumpImpulse = 0f) => _kcc.Move(velocity, jumpImpulse);
        public void AddLookRotation(Vector2 delta) => _kcc.AddLookRotation(delta);
        public Vector2 GetLookRotation(bool pitch = true, bool yaw = true) => _kcc.GetLookRotation(pitch, yaw);
        public bool ProjectOnGround(Vector3 vector, out Vector3 projectedVector) => _kcc.ProjectOnGround(vector, out projectedVector);
    }
}
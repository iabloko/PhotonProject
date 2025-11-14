using Sandbox.Project.Scripts.Infrastructure.ModelData.InteractionObjects;
using UnityEngine;

namespace Sandbox.Project.Scripts.Infrastructure.ModelData.MovementEffects
{
    [System.Serializable]
    public sealed class JumpPadMovementData : MovementEffectData
    {
        public readonly float NetJumpStrength;

        public JumpPadMovementData(
            InteractionObjectType type, Transform transform, Transform child, float netJumpStrength)
            : base(type, transform, child)
        {
            NetJumpStrength = netJumpStrength;
        }
    }

    [System.Serializable]
    public sealed class PortalMovementData : MovementEffectData
    {
        public readonly Vector3 EndPosition;

        public PortalMovementData(InteractionObjectType type, Transform transform, Transform child, Vector3 endPosition)
            : base(type, transform, child)
        {
            EndPosition = endPosition;
        }
    }

    [System.Serializable]
    public sealed class PopupMovementData : MovementEffectData
    {
        public PopupMovementData(InteractionObjectType type, Transform transform, Transform child) : base(type, transform, child)
        {
        }
    }

    [System.Serializable]
    public class MovementEffectData
    {
        public readonly InteractionObjectType Type;
        public readonly Transform Transform;
        public readonly Transform Child;

        public MovementEffectData(InteractionObjectType type, Transform transform, Transform child)
        {
            Type = type;
            Transform = transform;
            Child = child;
        }
    }
}
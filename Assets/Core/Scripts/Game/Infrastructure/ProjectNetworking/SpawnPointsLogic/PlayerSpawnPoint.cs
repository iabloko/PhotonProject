using Core.Scripts.Game.Infrastructure.ProjectNetworking.SpawnPointsLogic.Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.ProjectNetworking.SpawnPointsLogic
{
    public sealed class PlayerSpawnPoint : SpawnPoint
    {
        public Quaternion RotateToFaceDirection => Quaternion.Euler(GetDirectionWorld());
        public FacingDirections faceDirection;
        
        [SerializeField] private bool showGizmos;
        [SerializeField, ShowIf("showGizmos")] private int gizmoLength;
        [SerializeField, ShowIf("showGizmos")] private int gizmoRadius;
        
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;

            Vector3 origin = transform.position;
            Vector3 dir = GetDirectionWorld();

            Vector3 end = origin + dir * gizmoLength;

            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(origin, gizmoRadius);
            Gizmos.DrawLine(origin, end);

            float headSize = gizmoLength * 0.2f;
            Vector3 right = Vector3.Cross(dir, Vector3.up);
            
            if (right == Vector3.zero) right = Vector3.right;

            Vector3 headLeft = end - dir * headSize + right * headSize * 0.5f;
            Vector3 headRight = end - dir * headSize - right * headSize * 0.5f;

            Gizmos.DrawLine(end, headLeft);
            Gizmos.DrawLine(end, headRight);
        }

        private Vector3 GetDirectionWorld()
        {
            switch (faceDirection)
            {
                case FacingDirections.North:
                    return Vector3.forward;
                case FacingDirections.South:
                    return Vector3.back;
                case FacingDirections.East:
                    return Vector3.right;
                case FacingDirections.West:
                    return Vector3.left;
                default:
                    return Vector3.forward;
            }
        }
#endif
    }
}
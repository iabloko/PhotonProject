using UnityEngine;

namespace Core.Scripts.Game.PlayerLogic.Movement
{
    [System.Serializable]
    public struct PlayerTeleportationData
    {
        public Vector3 endPosition;
        public Quaternion endRotation;

        public PlayerTeleportationData(Vector3 endPosition, Quaternion endRotation)
        {
            this.endPosition = endPosition;
            this.endRotation = endRotation;
        }
    }
}
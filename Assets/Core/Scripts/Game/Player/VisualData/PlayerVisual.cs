using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.Player.VisualData
{
    [System.Serializable, HideLabel]
    public class PlayerVisual
    {
        public GameObject[] hair;
        public GameObject[] heads;
        public GameObject[] eyes;
        public GameObject[] mouth;
        public GameObject[] bodies;
    }

    [System.Serializable, BoxGroup("Player Visual Network"), HideLabel]
    public struct PlayerVisualNetwork : INetworkStruct
    {
        public int hairID;
        public int headID;
        public int eyeID;
        public int mountID;
        public int bodyID;

        public PlayerVisualNetwork(int hairID, int headID, int eyeID, int mountID, int bodyID)
        {
            this.hairID = hairID;
            this.headID = headID;
            this.eyeID = eyeID;
            this.mountID = mountID;
            this.bodyID = bodyID;
        }
    }
}
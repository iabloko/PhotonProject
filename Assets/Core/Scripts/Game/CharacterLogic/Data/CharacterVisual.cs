using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.CharacterLogic.Data
{
    [System.Serializable, HideLabel]
    public struct CharacterVisualNetwork : INetworkStruct
    {
        public int hairID;
        public int headID;
        public int eyeID;
        public int mountID;
        public int bodyID;

        public CharacterVisualNetwork(int hairID, int headID, int eyeID, int mountID, int bodyID)
        {
            this.hairID = hairID;
            this.headID = headID;
            this.eyeID = eyeID;
            this.mountID = mountID;
            this.bodyID = bodyID;
        }

        public override string ToString() => $"CharacterVisualNetwork Skin Data: {bodyID} | {hairID} | {headID} | {eyeID}";
    }
    
    [System.Serializable, HideLabel]
    public class CharacterVisual
    {
        public GameObject[] hair;
        public GameObject[] heads;
        public GameObject[] eyes;
        public GameObject[] mouth;
        public GameObject[] bodies;
    }
}
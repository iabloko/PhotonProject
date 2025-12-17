using Core.Scripts.Game.Infrastructure.ModelData.Room;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.ModelData
{
    [CreateAssetMenu(menuName = "Game/RoomSettings", fileName = "RoomSettings")]
    public sealed class RoomSettings : ScriptableObject
    {
        public PhotonRoomSettings settings;
    }
}
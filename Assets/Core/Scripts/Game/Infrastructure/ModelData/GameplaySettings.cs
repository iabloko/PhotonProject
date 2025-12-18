using Core.Scripts.Game.Infrastructure.ModelData.Room;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.ModelData
{
    [CreateAssetMenu(menuName = "Game/RoomSettings", fileName = "RoomSettings"), InlineEditor, HideLabel]
    public sealed class GameplaySettings : ScriptableObject
    {
        public PhotonRoomSettings settings;
    }
}
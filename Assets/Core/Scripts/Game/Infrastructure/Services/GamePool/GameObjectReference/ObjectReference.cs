using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.GamePool.GameObjectReference
{
    [CreateAssetMenu(fileName = "ObjectReference", menuName = "Game/Presets/ObjectReference")]
    public sealed class ObjectReference : ScriptableObject
    {
        [Title("Simple", "Objects", titleAlignment: TitleAlignments.Right)]
        public NetworkObject prototype;

        [Title("Interactable", "Objects", titleAlignment: TitleAlignments.Right)]
        public NetworkObject banana;
        public NetworkObject bubblegum;
        public NetworkObject check;
        public NetworkObject jump;
        public NetworkObject lava;
        public NetworkObject speed;
        public NetworkObject ascii;
        public NetworkObject lavaLine;
        public NetworkObject lavaPlane;
        public NetworkObject lavaWall;
        public NetworkObject primaryPortal;
        public NetworkObject secondaryPortal;
        public NetworkObject finish;
    }
}
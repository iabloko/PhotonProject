using UnityEngine;

namespace Sandbox.Project.Scripts.Player.PlayersSkins
{
    [CreateAssetMenu(fileName = "PlayerSkins", menuName = "Game/Presets/PlayerSkins")]
    public sealed class PlayerVisualData : ScriptableObject
    {
        public Texture2D[] skins;
    }
}
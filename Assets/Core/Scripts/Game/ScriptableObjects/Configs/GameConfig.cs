using Core.Scripts.Game.Infrastructure.ModelData;
using Core.Scripts.Game.ScriptableObjects.Configs.Logger;
using Core.Scripts.Game.ScriptableObjects.Settings;
using Fusion.Photon.Realtime;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace Core.Scripts.Game.ScriptableObjects.Configs
{
    [CreateAssetMenu(menuName = "Settings/Configs/GameConfig", fileName = "GameConfig")]
    public sealed partial class GameConfig : ScriptableObject
    {
        [Title("Log Settings", titleAlignment: TitleAlignments.Left), SerializeField]
        public GameLogger logger;
        
        [Title("Global Settings", titleAlignment: TitleAlignments.Left), SerializeField]
        public GameGlobalSettings globalSettings;        
        
        [Title("GameplaySettings Settings", titleAlignment: TitleAlignments.Left), SerializeField]
        public GameplaySettings gameplaySettings;        
        
        [Title("CinemachineBlender Settings", titleAlignment: TitleAlignments.Left), SerializeField, InlineEditor, HideLabel]
        public CinemachineBlenderSettings cinemachineBlenderSettings;
        
        [Title("Network Settings", titleAlignment: TitleAlignments.Left), SerializeField, InlineEditor, HideLabel]
        public PhotonAppSettings networkAppSettings;
    }
}
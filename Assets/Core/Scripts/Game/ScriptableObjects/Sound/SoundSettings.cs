using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Scripts.Game.ScriptableObjects.Sound
{
    [CreateAssetMenu(menuName = "Settings/Configs/SoundSettings", fileName = "SoundSettings")]
    public sealed class SoundSettings : ScriptableObject
    {
        [Required, InlineEditor(InlineEditorModes.LargePreview)] 
        public AudioClip clip;
        
        [ProgressBar(0, 1, r: 1, g: 1, b: 1, Height = 30)]
        public float volume;
        
        public AudioMixer mixer;
    }
}
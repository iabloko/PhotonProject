using Core.Scripts.Game.ScriptableObjects.Sound;
using UnityEngine;

namespace Core.Scripts.Game.ObjectDamageReceiver
{
    public sealed class SfxOnDamage
    {
        private readonly AudioSource _audioSource;
        private readonly SoundSettings _damageSoundSettings;

        public SfxOnDamage(AudioSource audioSource, SoundSettings damageSoundSettings)
        {
            _damageSoundSettings = damageSoundSettings;
            _audioSource = audioSource;

            _audioSource.clip = _damageSoundSettings.clip;
        }

        public void OnDamage(in DamageInfo info)
        {
            AudioClip c = _damageSoundSettings.clip;
            Debug.Log($"SfxOnDamage OnDamage {_damageSoundSettings}");
            Debug.Log($"SfxOnDamage OnDamage {_audioSource}");
            Debug.Log($"SfxOnDamage OnDamage {c}");
            _audioSource.volume = _damageSoundSettings.volume;
            _audioSource.Play();
        }
    }
}
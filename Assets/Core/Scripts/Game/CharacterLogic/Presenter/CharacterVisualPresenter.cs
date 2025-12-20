using Core.Scripts.Game.CharacterLogic.Data;
using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.CharacterLogic.Presenter
{
    public sealed class CharacterVisualPresenter
    {
        private const string DEFAULT_PLAYER_NICKNAME = "Player";

        private readonly CharacterVisual _visualData;

        public CharacterVisualPresenter(CharacterVisual visualData) => _visualData = visualData;

        public CharacterVisualNetwork CreateRandomVisual()
        {
            int hairId = Random.Range(0, _visualData.hair.Length);
            int headId = Random.Range(0, _visualData.heads.Length);
            int eyeId = Random.Range(0, _visualData.eyes.Length);
            int mouthId = Random.Range(0, _visualData.mouth.Length);
            int bodyId = Random.Range(0, _visualData.bodies.Length);

            return new CharacterVisualNetwork(hairId, headId, eyeId, mouthId, bodyId);
        }

        public NetworkString<_16> CreateDefaultNickname()
        {
            var networkString = new NetworkString<_16>();
            networkString.Set(DEFAULT_PLAYER_NICKNAME);
            return networkString;
        }

        public string FormatNickname(NetworkString<_16> playerNickname, NetworkId objectId)
        {
#if UNITY_EDITOR
            return $"{playerNickname.Value}_{objectId}";
#else
            return playerNickname.Value.ToString();
#endif
        }
    }
}
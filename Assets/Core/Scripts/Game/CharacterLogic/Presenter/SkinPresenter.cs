using Core.Scripts.Game.CharacterLogic.Data;

namespace Core.Scripts.Game.CharacterLogic.Presenter
{
    public sealed class SkinPresenter
    {
        private readonly CharacterVisual _data;

        public SkinPresenter(CharacterVisual data) => _data = data;

        public void Apply(CharacterVisualNetwork net)
        {
            for (int i = 0; i < _data.hair.Length; i++) _data.hair[i].SetActive(i == net.hairID);
            for (int i = 0; i < _data.heads.Length; i++) _data.heads[i].SetActive(i == net.headID);
            for (int i = 0; i < _data.eyes.Length; i++) _data.eyes[i].SetActive(i == net.eyeID);
            for (int i = 0; i < _data.mouth.Length; i++) _data.mouth[i].SetActive(i == net.mountID);
            for (int i = 0; i < _data.bodies.Length; i++) _data.bodies[i].SetActive(i == net.bodyID);
        }
    }
}
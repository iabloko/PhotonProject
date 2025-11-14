using UnityEngine;

namespace Core.Scripts.Game.InteractionObjects.Base
{
    public abstract class AnimatedInteractionObject : InteractionObject
    {
        [SerializeField] protected GameObject[] _gos;

        private float _timer;
        private int _selected = -1;

        protected virtual float AnimationDelay => 0.2f;
        
        protected virtual void Update()
        {
            _timer += Time.deltaTime;

            if (!(_timer >= AnimationDelay)) return;
            if (_selected == _gos.Length - 1) _selected = -1;
            _timer = 0;
            SelectNew(++_selected);
        }

        private void SelectNew(int index)
        {
            for (int i = 0; i < _gos.Length; i++)
            {
                _gos[i].SetActive(i == index);
            }
        }
    }
}
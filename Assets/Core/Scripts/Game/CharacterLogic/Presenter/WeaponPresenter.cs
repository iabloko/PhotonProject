using Core.Scripts.Game.GamePlay.UsableItems;

namespace Core.Scripts.Game.CharacterLogic.Presenter
{
    public sealed class WeaponPresenter
    {
        private readonly WeaponData[] _weaponData;
        private readonly CharacterAnimationPresenter _animation;

        public WeaponPresenter(WeaponData[] weaponData, CharacterAnimationPresenter animation)
        {
            _weaponData = weaponData;
            _animation = animation;
        }

        public void Apply(int weaponId)
        {
            for (int i = 0; i < _weaponData.Length; i++)
            {
                var visuals = _weaponData[i].visuals;
                for (int v = 0; v < visuals.Length; v++)
                    visuals[v].prefab.SetActive(false);
            }

            for (int i = 0; i < _weaponData.Length; i++)
            {
                if (_weaponData[i].weaponConfig.id != weaponId) continue;

                var visuals = _weaponData[i].visuals;
                for (int v = 0; v < visuals.Length; v++)
                    visuals[v].prefab.SetActive(true);

                _animation.OverrideAnimatorController(_weaponData[i].weaponConfig.weaponAnimations);
                return;
            }
        }
    }
}
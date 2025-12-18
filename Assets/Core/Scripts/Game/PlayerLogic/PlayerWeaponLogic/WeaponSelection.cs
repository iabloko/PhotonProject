using System;
using Core.Scripts.Game.Infrastructure.Services.Inventory;
using UniRx;

namespace Core.Scripts.Game.PlayerLogic.PlayerWeaponLogic
{
    public sealed class WeaponSelection : IDisposable
    {
        private readonly CompositeDisposable _cd = new();

        public WeaponSelection(IInventory inventory, Action<int> setWeaponId)
        {
            inventory.CurrentWeapon
                .Where(w => w != null)
                .Subscribe(w => setWeaponId(w.id))
                .AddTo(_cd);
        }

        public void Dispose() => _cd.Dispose();
    }
}
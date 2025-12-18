using Core.Scripts.Game.GameHelpers;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.Services.Inventory;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.States.Base;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Views;
using UnityEngine.Scripting;
using Zenject;

namespace Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.States
{
    public sealed class
        GameMenuUIGamePlayState : GameMenuUISimpleStateBase<GameMenuUIGamePlayView>
    {
        private readonly IInventory _inventory;

        public GameMenuUIGamePlayState(MainGameUIStateMachine stateMachine, IAssetProvider assetProvider, IInventory inventory) 
            : base(stateMachine, assetProvider)
        {
            _inventory = inventory;
        }

        public override string StateName => "GameMenuUIGamePlayState";
        protected override string ViewPath => GameConstant.GAME_UI_GAME_PLAY_VIEW;
        
        protected override void OnEntered()
        {
            base.OnEntered();
            _stateView.SetInventoryLogic(_inventory);
        }
        
        [Preserve]
        public class Factory : PlaceholderFactory<MainGameUIStateMachine, GameMenuUIGamePlayState>
        {
            [Preserve]
            public Factory()
            {
            }
        }
    }
}
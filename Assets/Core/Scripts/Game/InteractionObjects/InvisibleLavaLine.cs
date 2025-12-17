using Core.Scripts.Game.Infrastructure.ModelData.InteractionObjects;
using Core.Scripts.Game.InteractionObjects.Base;

namespace Core.Scripts.Game.InteractionObjects
{
    public sealed class InvisibleLavaLine : InteractionObject
    {
        public override bool IsInteractable => true;
        public override InteractionObjectType Type => InteractionObjectType.INVISIBLE_LAVA_LANE;

        public override void Interact()
        {
            if (IsInteractable.Equals(false)) return;
            base.Interact();
        }
    }
}
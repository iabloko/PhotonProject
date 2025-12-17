using Core.Scripts.Game.Infrastructure.ModelData.InteractionObjects;
using Core.Scripts.Game.InteractionObjects.Base;

namespace Core.Scripts.Game.InteractionObjects
{
    public sealed class LavaPlane : InteractionObject
    {
        public override bool IsInteractable => true;

        public override InteractionObjectType Type => InteractionObjectType.LAVA_PLANE;

        public override void Interact()
        {
            if (IsInteractable.Equals(false)) return;
            base.Interact();
        }
    }
}
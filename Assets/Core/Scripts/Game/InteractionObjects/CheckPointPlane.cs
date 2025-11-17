using Core.Scripts.Game.Infrastructure.ModelData.InteractionObjects;
using Core.Scripts.Game.InteractionObjects.Base;

namespace Core.Scripts.Game.InteractionObjects
{
    public sealed class CheckPointPlane : InteractionObject
    {
        public override bool IsInteractable => true;
        public override InteractionObjectType Type => InteractionObjectType.CHECK_POINT;

        public override void Interact()
        {
            if (IsInteractable.Equals(false) || WasInteracted.Equals(true) || DisabledAtStart) return;
            base.Interact();
        }
        
        public override void StopInteract()
        {
        }

        public override void EnableAtStart()
        {
            base.EnableAtStart();
            WasInteracted = false;
        }
    }
}
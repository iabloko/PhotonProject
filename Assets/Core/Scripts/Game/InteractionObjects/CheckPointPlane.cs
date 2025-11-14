using Core.Scripts.Game.InteractionObjects.Base;
using Sandbox.Project.Scripts.Infrastructure.ModelData.InteractionObjects;
using Sandbox.Project.Scripts.Infrastructure.ModelData.MovementEffects;

namespace Core.Scripts.Game.InteractionObjects
{
    public sealed class CheckPointPlane : InteractionObject
    {
        public override MovementEffectData EffectData { get; protected set; }
        public override bool IsInteractable => true;
        public override InteractionObjectType Type => InteractionObjectType.CHECK_POINT;
        
        public override void Spawned()
        {
            base.Spawned();
            EffectData = new MovementEffectData(Type, Transform, Child);
        }

        public override void Interact()
        {
            if (IsInteractable.Equals(false) || WasInteracted.Equals(true) || DisabledAtStart) return;
            // PlayerInfo.UpdateUserInteractionObject(Type, InteractionObjectIndex);
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
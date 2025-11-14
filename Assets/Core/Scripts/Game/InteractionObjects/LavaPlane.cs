using Core.Scripts.Game.InteractionObjects.Base;
using Sandbox.Project.Scripts.Infrastructure.ModelData.InteractionObjects;
using Sandbox.Project.Scripts.Infrastructure.ModelData.MovementEffects;

namespace Core.Scripts.Game.InteractionObjects
{
    public sealed class LavaPlane : InteractionObject
    {
        public override bool IsInteractable => true;

        public override InteractionObjectType Type => InteractionObjectType.LAVA_PLANE;
        public override MovementEffectData EffectData { get; protected set; }

        public override void Spawned()
        {
            base.Spawned();
            EffectData = new MovementEffectData(Type, Transform, Child);
        }

        public override void Interact()
        {
            if (IsInteractable.Equals(false)) return;
            base.Interact();
        }
    }
}
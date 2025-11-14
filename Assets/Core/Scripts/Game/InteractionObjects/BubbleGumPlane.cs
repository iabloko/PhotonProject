using Core.Scripts.Game.InteractionObjects.Base;
using Sandbox.Project.Scripts.Infrastructure.ModelData.InteractionObjects;
using Sandbox.Project.Scripts.Infrastructure.ModelData.MovementEffects;
using UnityEngine;

namespace Core.Scripts.Game.InteractionObjects
{
    public sealed class BubbleGumPlane : InteractionObject
    {
        public override MovementEffectData EffectData { get; protected set; }

        private const float INTERACTION_DELAY = 5f;
        public override bool IsInteractable => (Time.time > _lastInteractionTime + INTERACTION_DELAY);

        private float _lastInteractionTime = float.MinValue;
        public override InteractionObjectType Type => InteractionObjectType.BUBBLE_GUM;
        
        public override void Spawned()
        {
            base.Spawned();
            EffectData = new MovementEffectData(Type, Transform, Child);
        }
        
        public override void Interact()
        {
            if (IsInteractable.Equals(false)) return;
            
            _lastInteractionTime = Time.time;

            base.Interact();
        }
    }
}
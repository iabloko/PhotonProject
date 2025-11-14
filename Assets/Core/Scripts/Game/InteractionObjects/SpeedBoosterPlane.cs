using Core.Scripts.Game.InteractionObjects.Base;
using Sandbox.Project.Scripts.Infrastructure.ModelData.InteractionObjects;
using Sandbox.Project.Scripts.Infrastructure.ModelData.MovementEffects;
using UnityEngine;

namespace Core.Scripts.Game.InteractionObjects
{
    public sealed class SpeedBoosterPlane : InteractionObject
    {
        // TODO Need a common variable for SpeedBoosterPlane and SpeedUpSpecialObjectEffect
        private const float INTERACTION_DELAY = 3.5f;
        // We need this check to avoid writing additional ticks to the replay
        public override bool IsInteractable => (Time.time > _lastInteractionTime + INTERACTION_DELAY);

        private float _lastInteractionTime = float.MinValue;

        public override InteractionObjectType Type => InteractionObjectType.SPEED_BOOSTER_PLANE;
        public override MovementEffectData EffectData { get; protected set; }

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
using Core.Scripts.Game.Infrastructure.ModelData.InteractionObjects;
using Core.Scripts.Game.InteractionObjects.Base;
using UnityEngine;

namespace Core.Scripts.Game.InteractionObjects
{
    public sealed class SpeedBoosterPlane : InteractionObject
    {
        private const float INTERACTION_DELAY = 3.5f;
        public override bool IsInteractable => (Time.time > _lastInteractionTime + INTERACTION_DELAY);

        private float _lastInteractionTime = float.MinValue;

        public override InteractionObjectType Type => InteractionObjectType.SPEED_BOOSTER_PLANE;

        public override void Interact()
        {
            if (IsInteractable.Equals(false)) return;
            _lastInteractionTime = Time.time;
            base.Interact();
        }
    }
}
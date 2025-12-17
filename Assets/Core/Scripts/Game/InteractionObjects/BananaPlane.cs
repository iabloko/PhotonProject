using Core.Scripts.Game.Infrastructure.ModelData.InteractionObjects;
using Core.Scripts.Game.InteractionObjects.Base;
using UnityEngine;

namespace Core.Scripts.Game.InteractionObjects
{
    public sealed class BananaPlane : InteractionObject
    {
        public override InteractionObjectType Type => InteractionObjectType.BANANA;

        private const float INTERACTION_DELAY = 3f;
        public override bool IsInteractable => (Time.time > _lastInteractionTime + INTERACTION_DELAY);

        private float _lastInteractionTime = float.MinValue;

        public override void Interact()
        {
            if (IsInteractable.Equals(false)) return;

            _lastInteractionTime = Time.time;
            base.Interact();
        }
    }
}
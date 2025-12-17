using Core.Scripts.Game.Infrastructure.ModelData.InteractionObjects;
using Core.Scripts.Game.InteractionObjects.Base;
using UnityEngine;

namespace Core.Scripts.Game.InteractionObjects
{
    public sealed class DeathPlane : InteractionObject
    {
        [SerializeField] private Transform deathPlaneEffectSize;
        public override bool IsInteractable => true;
        public override InteractionObjectType Type => InteractionObjectType.DEATH_PLANE;

        public override void Interact()
        {
            if (IsInteractable.Equals(false)) return;
            base.Interact();
        }
    }
}
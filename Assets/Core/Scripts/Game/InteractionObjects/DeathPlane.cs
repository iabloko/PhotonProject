using Core.Scripts.Game.InteractionObjects.Base;
using Sandbox.Project.Scripts.Infrastructure.ModelData.InteractionObjects;
using Sandbox.Project.Scripts.Infrastructure.ModelData.MovementEffects;
using UnityEngine;

namespace Core.Scripts.Game.InteractionObjects
{
    public sealed class DeathPlane : InteractionObject
    {
        [SerializeField] private Transform deathPlaneEffectSize;
        public override bool IsInteractable => true;
        public override InteractionObjectType Type => InteractionObjectType.DEATH_PLANE;
        public override MovementEffectData EffectData { get; protected set; }
        
        public override void Spawned()
        {
            base.Spawned();
            EffectData = new MovementEffectData(Type, deathPlaneEffectSize, Child);
        }

        public override void Interact()
        {
            if (IsInteractable.Equals(false)) return;
            base.Interact();
        }
    }
}
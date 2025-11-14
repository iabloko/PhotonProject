using Sandbox.Project.Scripts.Infrastructure.ModelData.InteractionObjects;
using Sandbox.Project.Scripts.Infrastructure.ModelData.MovementEffects;
using UnityEngine;

namespace Core.Scripts.Game.InteractionObjects.Base
{
    public interface ISpecialPlane
    {
        public MovementEffectData EffectData { get; }
        public Transform Transform { get; }
        public InteractionObjectType Type { get; }
        public int InteractionObjectIndex { get; set; }
        public bool IsInteractable { get; }
        public bool DisabledAtStart { get; }

        public void SetInteractionObjectIndex(int index);
        public void Interact();
        public void StopInteract();
        public void DisableAtStart();
        public void EnableAtStart();
    }
}
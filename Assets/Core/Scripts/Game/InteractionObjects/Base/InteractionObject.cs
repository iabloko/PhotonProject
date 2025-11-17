using Core.Scripts.Game.Infrastructure.ModelData.InteractionObjects;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.InteractionObjects.Base
{
    public abstract class InteractionObject : NetworkBehaviour, ISpecialPlane
    {
        public Transform Transform => transform;
        public Transform Child => transform.GetChild(0);

        [ShowInInspector] public int InteractionObjectIndex { get; set; }

        [ShowInInspector] protected bool WasInteracted { get; set; }

        [ShowInInspector] public bool DisabledAtStart { get; private set; }

        public abstract bool IsInteractable { get; }
        public abstract InteractionObjectType Type { get; }

        
        public override void Spawned() => DisabledAtStart = false;


        public void SetInteractionObjectIndex(int index) => InteractionObjectIndex = index;

        public virtual void Interact() => WasInteracted = true;

        public virtual void StopInteract() => WasInteracted = false;
        public void DisableAtStart() => DisabledAtStart = true;
        public virtual void EnableAtStart() => DisabledAtStart = false;
    }
}
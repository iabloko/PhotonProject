using Fusion;
using Sandbox.Project.Scripts.Infrastructure.ModelData.InteractionObjects;
using Sandbox.Project.Scripts.Infrastructure.ModelData.MovementEffects;
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

        public abstract MovementEffectData EffectData { get; protected set; }
        public abstract bool IsInteractable { get; }
        public abstract InteractionObjectType Type { get; }

        protected IPlayerMovementEffectsListener _listener;
        
        public override void Spawned() => DisabledAtStart = false;

        public void Constructor(IPlayerMovementEffectsListener listener) => _listener = listener;

        public void SetInteractionObjectIndex(int index) => InteractionObjectIndex = index;

        public virtual void Interact()
        {
            WasInteracted = true;
            _listener.StartMovementEffect(EffectData);
        }

        public virtual void StopInteract() => WasInteracted = false;
        public void DisableAtStart() => DisabledAtStart = true;
        public virtual void EnableAtStart() => DisabledAtStart = false;
    }
}
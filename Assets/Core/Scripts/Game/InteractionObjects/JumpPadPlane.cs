using Core.Scripts.Game.Infrastructure.ModelData.InteractionObjects;
using Core.Scripts.Game.InteractionObjects.Base;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.InteractionObjects
{
    public struct JumpPadProperties
    {
        [JsonProperty("jumpStren")] public float JumpPadStrength;
    }

    public sealed class JumpPadPlane : InteractionObject
    {
        [ShowInInspector] public float NetJumpStrength { get; private set; }

        [ShowInInspector]
        public JumpPadProperties Properties
        {
            get => _properties;
            set
            {
                _properties = value;
                NetJumpStrength = value.JumpPadStrength;
            }
        }

        public override bool IsInteractable => true;
        public override InteractionObjectType Type => InteractionObjectType.JUMP_PAD;

        private JumpPadProperties _properties;


        public override void Interact()
        {
            if (IsInteractable.Equals(false)) return;
            base.Interact();
        }

#if UNITY_EDITOR

        [SerializeField] private bool onDrawGizmos;

        private void OnDrawGizmos()
        {
            if (onDrawGizmos.Equals(false)) return;
            IsBotOnGroundGizmo();
        }

        private void IsBotOnGroundGizmo()
        {
            Gizmos.color = Color.red;
            Vector3 forward = Child.forward;
            forward = NetJumpStrength > 0 ? forward * NetJumpStrength : forward;
            Gizmos.DrawRay(transform.localPosition, forward);
        }

#endif
    }
}
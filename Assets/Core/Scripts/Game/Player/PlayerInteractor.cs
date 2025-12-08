using System.Collections.Generic;
using Core.Scripts.Game.Player.NetworkInput;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.Player
{
    public abstract class PlayerInteractor : PlayerNetworkBehaviour
    {
        [Title("Interactor", subtitle: "Draggable", TitleAlignments.Right), SerializeField, MinValue(0.5f)]
        private float interactionDistance = 6f;
        [SerializeField] private LayerMask hoverMask;

        private Vector3 _currentRayPosition;
        private Vector3 _grabLocalOffset;

        private bool _hasGrab;
        private int _layerOutline;
        private float _maxInteractionDistance;

        private readonly List<(GameObject go, int layer)> _savedLayers = new(32);
        private static readonly List<Renderer> RendererCache = new(32);
        private Transform _hoverRoot;

        public override void Spawned()
        {
            base.Spawned();
            _layerOutline = LayerMask.NameToLayer(hoverMask.ToString());
        }
        
        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            
            if (!Object.HasStateAuthority) return;
            
            // InputModelData curr = input.CurrentInput;
            // InputModelData prev = input.PreviousInput;

            // InteractWithObjects(curr, prev);
        }

        private void InteractWithObjects(InputModelData curr, InputModelData prev)
        {
            // bool pressed = curr.Actions.WasPressed(prev.Actions, InputModelData.DRAG_BUTTON);
            // bool held = curr.Actions.IsSet(InputModelData.DRAG_BUTTON);
            // bool released = curr.Actions.WasReleased(prev.Actions, InputModelData.DRAG_BUTTON);
            // bool copy = curr.Actions.WasPressed(prev.Actions, InputModelData.COPY_BUTTON);
            // bool scaleHeld = curr.Actions.IsSet(InputModelData.SCALE_BUTTON);

            // if (pressed) PrepareToDrag(); 
            // if (held) TickDrag();
            // if (released) EndDrag();
        }
    }
}
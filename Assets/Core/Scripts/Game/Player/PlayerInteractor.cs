using System.Collections.Generic;
using Core.Scripts.Game.Infrastructure.Services.Cinemachine;
using Core.Scripts.Game.Player.NetworkInput;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.Player
{
    public abstract class PlayerInteractor : PlayerNetworkBehaviour
    {
        [Title("Interactor", subtitle: "Draggable", TitleAlignments.Right), SerializeField, MinValue(0.5f)]
        private float interactionDistance = 6f;
        private float _maxInteractionDistance;
        private Vector3 _currentRayPosition;

        [Title("Interactor", subtitle: "Outline", TitleAlignments.Right), SerializeField]
        private LayerMask hoverMask;

        private int _layerOutline;

        private Vector3 _grabLocalOffset;
        private bool _hasGrab;

        private readonly List<(GameObject go, int layer)> _savedLayers = new(32);
        private static readonly List<Renderer> RendererCache = new(32);
        private Transform _hoverRoot;

        public override void Spawned()
        {
            base.Spawned();
            _layerOutline = LayerMask.NameToLayer(hoverMask.ToString());
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            ClearOutline();
        }

        public override void BeforeTick()
        {
            if (!Object.HasStateAuthority) return;

            InputModelData curr = input.CurrentInput;
            InputModelData prev = input.PreviousInput;

            InteractWithObjects(curr, prev);
        }

        private void InteractWithObjects(InputModelData curr, InputModelData prev)
        {
            bool pressed = curr.Actions.WasPressed(prev.Actions, InputModelData.DRAG_BUTTON);
            bool held = curr.Actions.IsSet(InputModelData.DRAG_BUTTON);
            bool released = curr.Actions.WasReleased(prev.Actions, InputModelData.DRAG_BUTTON);
            bool copy = curr.Actions.WasPressed(prev.Actions, InputModelData.COPY_BUTTON);
            bool scaleHeld = curr.Actions.IsSet(InputModelData.SCALE_BUTTON);
            bool emptyBackPack = curr.Actions.IsSet(InputModelData.EMPTY_BACKPACK_BUTTON);

            // if (pressed) PrepareToDrag();
            // if (held) TickDrag();
            // if (released) EndDrag();
        }

        private void PrepareToDrag()
        {
            if (Cinemachine.CurrentState is CinemachineState.Preview or CinemachineState.Teleportation) return;

            Vector3 origin = ViewOrigin();
            Vector3 fwd = ViewForward();

            PhysicsScene scene = Runner.GetPhysicsScene();

            _maxInteractionDistance = ComputeMaxPickDistance();

            // if (!scene.Raycast(
            //         origin,
            //         direction: fwd,
            //         out RaycastHit hit,
            //         _maxInteractionDistance,
            //         draggableMask,
            //         QueryTriggerInteraction.Ignore)) return;

            // _currentHit.collider.TryGetComponent(out NetworkDraggable d);
            //
            // if (d == null)
            // {
            //     CreateProp(_currentHit.transform.position).Forget();
            // }
            // else
            // {
            //     BeginDrag(d);
            // }
        }

        // private async UniTaskVoid Copy()
        // {
        //     Vector3 scale = (_currentDraggable != null) ? _currentDraggable.transform.localScale : Vector3.one;
        //     await NetworkPool.DuplicateObject<NetworkObject>(
        //         _currentId, _currentRayPosition, Quaternion.identity, scale, string.Empty);
        // }
        //
        // private async UniTaskVoid CreateProp(Vector3 position)
        // {
        //     NetworkDraggable draggable = await NetworkPool.DuplicateObject<NetworkDraggable>(
        //         _currentId, position, Quaternion.identity, Vector3.one, string.Empty);
        //     BeginDrag(draggable);
        // }
        //
        // private void TickScale(float horizontalDelta)
        // {
        //     if (_currentDraggable == null ||
        //         !_currentDraggable.IsDragged || 
        //         !_currentDraggable.Object.HasStateAuthority) return;
        //
        //     if (Mathf.Abs(horizontalDelta) < Mathf.Epsilon) return;
        //
        //     Transform t = _currentDraggable.transform;
        //
        //     Vector3 s = t.localScale;
        //     float avg = (Mathf.Abs(s.x) + Mathf.Abs(s.y) + Mathf.Abs(s.z)) / 3f;
        //     if (avg < Mathf.Epsilon) avg = Mathf.Epsilon;
        //
        //     float factorRaw = 1f + horizontalDelta * scaleSensitivity;
        //     float desiredAvg = Mathf.Clamp(avg * factorRaw, minUniformScale, maxUniformScale);
        //     float finalFactor = desiredAvg / avg;
        //
        //     t.localScale = s * finalFactor;
        //
        //     _dragDistance = Mathf.Max(_dragDistance, minDragDistance);
        //
        //     if (_hasGrab)
        //     {
        //         Vector3 camPos = MainCamera.transform.position;
        //         Vector3 fwd = ViewForward();
        //
        //         Vector3 targetGrabPoint = camPos + fwd * _dragDistance;
        //         Vector3 pivotTarget = targetGrabPoint - t.TransformVector(_grabLocalOffset);
        //
        //         _currentRayPosition = pivotTarget;
        //         _currentDraggable.DriveTo(pivotTarget);
        //     }
        //     else
        //     {
        //         Vector3 target = MainCamera.transform.position + ViewForward() * _dragDistance;
        //         _currentRayPosition = target;
        //         _currentDraggable.DriveTo(target);
        //     }
        // }
        //
        // private void BeginDrag(NetworkDraggable d)
        // {
        //     _currentDraggable = d;
        //
        //     Transform t = _currentDraggable.transform;
        //
        //     _grabLocalOffset = t.InverseTransformPoint(_currentHit.point);
        //     _hasGrab = true;
        //
        //     float surfaceDist = _currentHit.distance > 0f ? _currentHit.distance : 2f;
        //     _dragDistance = Mathf.Max(surfaceDist, minDragDistance);
        //
        //     Vector3 camPos = MainCamera.transform.position;
        //     Vector3 fwd = ViewForward();
        //     Vector3 targetGrabPoint = camPos + fwd * _dragDistance;
        //     Vector3 pivotTarget = targetGrabPoint - t.TransformVector(_grabLocalOffset);
        //     _currentRayPosition = pivotTarget;
        //
        //     if (_currentDraggable.Object.HasStateAuthority)
        //     {
        //         PlayerInfo.PlayerInStatus.ChangeDraggingStatus(true);
        //         _currentDraggable.RPC_RequestBeginDrag(Object.InputAuthority, _dragDistance);
        //         _currentDraggable.DriveTo(pivotTarget);
        //     }
        //     else
        //     {
        //         _currentDraggable.Object.RequestStateAuthority();
        //     }
        // }
        //
        // private void TickDrag()
        // {
        //     if (_currentDraggable == null || !_currentDraggable.IsDragged) return;
        //
        //     _dragDistance = Mathf.Max(_dragDistance, minDragDistance);
        //
        //     Transform t = _currentDraggable.transform;
        //     Vector3 camPos = MainCamera.transform.position;
        //     Vector3 fwd = ViewForward();
        //
        //     Vector3 targetGrabPoint = camPos + fwd * _dragDistance;
        //     Vector3 pivotTarget = targetGrabPoint - t.TransformVector(_grabLocalOffset);
        //
        //     _currentRayPosition = pivotTarget;
        //     _currentDraggable.DriveTo(pivotTarget);
        // }
        //
        // private void EndDrag()
        // {
        //     if (_currentDraggable == null) return;
        //
        //     PlayerInfo.PlayerInStatus.ChangeDraggingStatus(false);
        //
        //     _currentDraggable = null;
        //     _hasGrab = false;
        //     _grabLocalOffset = default;
        // }

        private Vector3 ViewForward(bool usePitch = true)
        {
            if (MainCamera != null)
            {
                Vector3 fwd = MainCamera.transform.forward;
                if (!usePitch) fwd.y = 0f;
                return fwd.sqrMagnitude > 1e-6f ? fwd.normalized : transform.forward;
            }

            Vector2 pitchYaw = kcc.GetLookRotation(pitch: true, yaw: true);
            Quaternion q = Quaternion.Euler(pitchYaw.x, pitchYaw.y, 0f);
            Vector3 kccFwd = q * Vector3.forward;
            if (!usePitch) kccFwd.y = 0f;
            return kccFwd.sqrMagnitude > 1e-6f ? kccFwd.normalized : transform.forward;
        }

        private Vector3 ViewOrigin(float eyeHeight = 1.6f)
        {
            if (MainCamera != null) return MainCamera.transform.position;
            return transform.position + Vector3.up * eyeHeight;
        }

        #region OUTLINE LOGIC

        public override void Render()
        {
            base.Render();

            if (!Object.HasInputAuthority) return;

            if (Cinemachine.CurrentState is CinemachineState.Preview or CinemachineState.Teleportation)
            {
                ClearOutline();
                return;
            }

            Vector3 origin = ViewOrigin();
            Vector3 fwd = ViewForward();

            PhysicsScene scene = Runner.GetPhysicsScene();

            // _maxPickDistance = Cinemachine.CurrentCameraDistance + pickDistance;
            _maxInteractionDistance = ComputeMaxPickDistance();

            if (scene.Raycast(origin, fwd, out RaycastHit hit, _maxInteractionDistance, hoverMask,
                    QueryTriggerInteraction.Ignore))
            {
                Transform newRoot = hit.collider.transform;

                if (_hoverRoot == newRoot) return;

                ClearOutline();
                ApplyOutline(newRoot);
            }
            else
            {
                ClearOutline();
            }
        }

        private void ApplyOutline(Transform root)
        {
            _hoverRoot = root;

            RendererCache.Clear();
            root.GetComponentsInChildren(RendererCache);

            foreach (Renderer r in RendererCache)
            {
                GameObject go = r.gameObject;
                _savedLayers.Add((go, go.layer));
                go.layer = _layerOutline;
            }
        }

        private void ClearOutline()
        {
            if (_savedLayers.Count == 0) return;

            foreach ((GameObject go, int layer) in _savedLayers)
            {
                if (go) go.layer = layer;
            }

            _savedLayers.Clear();
            _hoverRoot = null;
        }

        private float ComputeMaxPickDistance()
        {
            // float camToPlayer = (MainCamera != null) 
            //     ? Vector3.Distance(MainCamera.transform.position, transform.position)
            //     : Cinemachine.CurrentCameraDistance;            

            const float hardCap = 100f;
            return Mathf.Min(Cinemachine.CurrentCameraDistance + interactionDistance, hardCap);
        }

        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 origin = Application.isPlaying ? ViewOrigin() : transform.position + Vector3.up * 1.6f;
            Vector3 fwd = Application.isPlaying
                ? ViewForward()
                : new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + fwd * _maxInteractionDistance);
            Gizmos.DrawSphere(origin, 0.05f);
            Gizmos.DrawSphere(origin + fwd * _maxInteractionDistance, 0.06f);
        }
#endif
    }
}
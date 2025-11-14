using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;

namespace Core.Scripts.Game.Player.HitBoxCollision
{
    public sealed class PlayerCollision : NetworkBehaviour
    {
        [SerializeField] private SimpleKCC KCC;

        private const string PROPS = "Props";
        private const string PORTAL = "Portal";
        private const string DEATH_PROPS = "DeathProps";
        private const string FINISH = "Finish";
        
        private void OnTriggerEnter(Collider hit)
        {
            if (!Object.HasStateAuthority) return;

            // if (hit.gameObject.CompareTag(DEATH_PROPS))
            // {
            //     if (hit.gameObject.TryGetComponent(out ISpecialPlane specialPlane))
            //     {
            //         specialPlane.Interact();
            //     }
            // }
            // else if (hit.gameObject.CompareTag(PORTAL))
            // {
            //     if (hit.gameObject.TryGetComponent(out Portal portal))
            //     {
            //         portal.OnPlayerTrigger();
            //     }
            // }
            // else if (hit.gameObject.CompareTag(FINISH))
            // {
            //     // _gameProgress.FinishGame();
            // }
            // else if (hit.gameObject.CompareTag(PROPS) && KCC.IsGrounded)
            // {
            //     if (hit.gameObject.TryGetComponent(out ISpecialPlane specialPlane))
            //     {
            //         specialPlane.Interact();
            //     }
            // }
        }

        private void OnTriggerExit(Collider hit)
        {
            // if (hit.TryGetComponent(out ISpecialPlane specialPlane))
            // {
            //     specialPlane.StopInteract();
            // }
        }
    }
}
using CozySpringJam.Game.Objects;
using UnityEngine;

namespace CozySpringJam.Game.Player
{
    public class PlayerAvatarInteract : MonoBehaviour
    {
        [SerializeField] private Transform _view;

        public void CheckEnvironment()
        {
            if (ShootRay(_view.forward, 2f, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out IInteractable interactableObject))
                    interactableObject.Interact();
            }
        }

        private bool ShootRay(Vector3 direction, float distance, out RaycastHit hit)
        {
            var origin = transform.position + Vector3.up;
            Ray ray = new Ray(origin, direction.normalized);

            Debug.DrawRay(origin, direction.normalized * distance, Color.red, 1f);

            int layerMask = 1 << 7;

            return Physics.Raycast(ray, out hit, distance, layerMask);
        }
    }
}

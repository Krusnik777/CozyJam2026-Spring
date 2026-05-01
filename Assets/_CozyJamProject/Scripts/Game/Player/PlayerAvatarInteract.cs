using UnityEngine;

namespace CozySpringJam.Game.Player
{
    public class PlayerAvatarInteract : MonoBehaviour
    {
        [SerializeField] private Transform _view;
        private void Update()
        {
            if(Input.GetButtonDown("Jump"))
                if (ShootRay(_view.forward, 2f, out RaycastHit hit))
                {
                    if (hit.collider.TryGetComponent(out IInteractable interactableObject))
                        interactableObject.Interact();
                }
        }
        
        private bool ShootRay(Vector3 direction, float distance, out RaycastHit hit)
        {
            var origin = transform.position + new Vector3(0,1,0);
            Ray ray = new Ray(origin, direction.normalized);

            Debug.DrawRay(origin, direction.normalized * distance, Color.red, 1f);

            return Physics.Raycast(ray, out hit, distance);
        }
    }
}

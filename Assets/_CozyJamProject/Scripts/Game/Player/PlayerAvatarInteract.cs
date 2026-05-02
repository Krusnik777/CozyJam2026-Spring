using CozySpringJam.Game.Objects;
using R3;
using UnityEngine;

namespace CozySpringJam.Game.Player
{
    public class PlayerAvatarInteract : MonoBehaviour
    {
        [SerializeField] private Transform _view;
        [SerializeField] private PlayerAvatarAnimator _playerAvatarAnimator;

        public Observable<IInteractable> DetectedInteractable => _detectedInteractableObject;
        private ReactiveProperty<IInteractable> _detectedInteractableObject = new();

        public void CheckEnvironment()
        {
            if (_detectedInteractableObject.Value == null) return;

            _detectedInteractableObject.Value.Interact();
            _playerAvatarAnimator.Interact();
        }

        private void Update()
        {
            if (ShootRay(_view.forward, 2f, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out IInteractable interactableObject))
                {
                    if (interactableObject.IsAvailableForInteraction)
                    {
                        if (_detectedInteractableObject.Value != interactableObject)
                            _detectedInteractableObject.Value = interactableObject;

                        return;
                    }
                }
            }

            if (_detectedInteractableObject.Value != null) _detectedInteractableObject.Value = null;
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

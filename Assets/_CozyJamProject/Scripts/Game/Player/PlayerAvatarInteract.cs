using System;
using CozySpringJam.Game.Objects;
using CozySpringJam.Game.Services;
using R3;
using UnityEngine;

namespace CozySpringJam.Game.Player
{
    public class PlayerAvatarInteract : MonoBehaviour, IDisposable
    {
        [SerializeField] private Transform _view;
        [SerializeField] private PlayerAvatarAnimator _playerAvatarAnimator;
        [SerializeField] private ControlsTip _interactTip;

        public Observable<IInteractable> DetectedInteractable => _detectedInteractableObject;
        private ReactiveProperty<IInteractable> _detectedInteractableObject = new();

        private GameInputService _gameInputService;

        private IDisposable _disposable;

        public void Bind(GameInputService gameInputService)
        {
            _gameInputService = gameInputService; 
            _gameInputService.SetOnPlayerInteract(CheckEnvironment);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _interactTip.Unsubscribe();
        }

        private void Awake()
        {           
            _disposable?.Dispose(); // just to be safe
            _disposable = DetectedInteractable.Subscribe(detected => _interactTip.gameObject.SetActive(detected != null));

            _interactTip.Subscribe();
        }

        private void OnDestroy()
        {
            Dispose();
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

        private void CheckEnvironment()
        {
            if (_detectedInteractableObject.Value == null) return;

            _detectedInteractableObject.Value.Interact();
            _playerAvatarAnimator.Interact();
        }
    }
}

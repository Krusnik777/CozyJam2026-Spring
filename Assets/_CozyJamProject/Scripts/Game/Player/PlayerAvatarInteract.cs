using System;
using CozySpringJam.Game.Objects;
using CozySpringJam.Game.Services;
using R3;
using UnityEngine;

namespace CozySpringJam.Game.Player
{
    public class PlayerAvatarInteract : MonoBehaviour, IDisposable
    {
        [Tooltip("0 - is center ray")][SerializeField] private Transform[] _raysOrigins;
        [SerializeField] private float _raycastDistance = 2f;
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
            if (_gameInputService != null) _gameInputService.SetOnPlayerInteract(null);
        }

        private void Awake()
        {   
            if (_raysOrigins.Length == 0)
            {
                Debug.LogError("Rays is empty - fix it");
                enabled = false;
                return;
            }

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
            //if (ShootRay(_view.forward, _raycastDistance, out RaycastHit hit))
            if (ShootRays(out RaycastHit hit))
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

        private bool ShootRays(out RaycastHit centerHit)
        {
            var centerRayOrigin = _raysOrigins[0].position;
            Ray centerRay = new Ray(centerRayOrigin, _raysOrigins[0].forward.normalized);

            int layerMask = 1 << 7;

            bool result = Physics.Raycast(centerRay, out centerHit, _raycastDistance, layerMask);

            bool neighbourChecksIsSame = true;

            for (int i = 1; i < _raysOrigins.Length; i++)
            {
                var neighbourOrigin = _raysOrigins[i].position;
                Ray neighbourRay = new Ray(neighbourOrigin, _raysOrigins[i].forward.normalized);
                RaycastHit neighbourHit;
                Physics.Raycast(neighbourRay, out neighbourHit, _raycastDistance, layerMask);

                neighbourChecksIsSame = neighbourHit.collider == centerHit.collider;

                if (!neighbourChecksIsSame) break;
            }

            return result && neighbourChecksIsSame;
        }

        private void CheckEnvironment()
        {
            if (_detectedInteractableObject.Value == null) return;

            _detectedInteractableObject.Value.Interact();
            _playerAvatarAnimator.Interact();
        }

        #if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            foreach (var rayOrigin in _raysOrigins)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(rayOrigin.position, rayOrigin.position + rayOrigin.forward * _raycastDistance);
            }
        }

        #endif
    }
}

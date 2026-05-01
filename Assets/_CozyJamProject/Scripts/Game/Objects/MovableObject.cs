using System;
using DG.Tweening;
using R3;
using UnityEngine;

namespace CozySpringJam
{
    public class MovableObject : MonoBehaviour, IInteractable
    {
        [Header("References")]
        [SerializeField] private InteractZone _upTrigger;
        [SerializeField] private InteractZone _rightTrigger;
        [SerializeField] private InteractZone _downTrigger;
        [SerializeField] private InteractZone _leftTrigger;
        
        [Header("Settings")]
        [SerializeField] private float _moveDuration;
        [SerializeField] private float _scalePowerAnimation;
        private float _baseScale;
        private Vector3 _direction = Vector3.zero;
        private bool _isMoving = false;
        private CompositeDisposable  _disposables = new();

        private void Start()
        {
            _disposables.Add(_leftTrigger.OnEnter.Subscribe(_ =>
            {
                ChangeDirectionMove(new Vector2(2, 0));
                Interact();
            }));
            _disposables.Add(_rightTrigger.OnEnter.Subscribe(_ =>
            {
                ChangeDirectionMove(new Vector2(-2, 0));
                Interact();
            }));
            _disposables.Add(_upTrigger.OnEnter.Subscribe(_ =>
            {
                ChangeDirectionMove(new Vector2(0, -2));
                Interact();
            }));
            _disposables.Add(_downTrigger.OnEnter.Subscribe(_ =>
            {
                ChangeDirectionMove(new Vector2(0, 2));
                Interact();
            }));
            
            _baseScale = transform.localScale.x;
        }
        
        public void Interact()
        {
            if(TryMoveToDirection())
                Move();
        }
        
        private void ChangeDirectionMove(Vector2 direction)
        {
            _direction = new Vector3(direction.x, 0, direction.y);
            Debug.Log(_direction);
        }
        
        private bool TryMoveToDirection()
        {
            if (ShootRay(_direction, 0.5f, out RaycastHit hit))
            {
                return false;
            }
            return true;
        }
        
        private bool ShootRay(Vector3 direction, float distance, out RaycastHit hit)
        {
            var origin = transform.position + new Vector3(0,1,0) + direction / 1.9f;
            Ray ray = new Ray(origin, direction.normalized);

            Debug.DrawRay(origin, direction.normalized * distance, Color.red, 1f);

            return Physics.Raycast(ray, out hit, distance);
        }
        
        #region Move
        private void Move()
        {
            _isMoving = true;
            Vector3 targetPosition = transform.position + _direction; 
            transform.DOMove(targetPosition, _moveDuration).SetEase(Ease.InFlash).OnComplete(FinishMove);
            MoveAnimation();
        }
        
        private void MoveAnimation()
        {
            var seq = DOTween.Sequence();
            if (_direction.x != 0)
            {
                seq.Append(transform.DOScaleX(_scalePowerAnimation, _moveDuration / 2)
                    .SetEase(Ease.OutQuad));
                seq.Join(transform.DOScaleY(_scalePowerAnimation, _moveDuration / 2)
                    .SetEase(Ease.OutQuad));
                seq.Join(transform.DOScaleZ(_baseScale - (_scalePowerAnimation - _baseScale), _moveDuration / 2)
                    .SetEase(Ease.OutQuad));
                
                seq.Append(transform.DOScaleX(_baseScale, _moveDuration / 2)
                    .SetEase(Ease.InQuad));
                seq.Join(transform.DOScaleY(_baseScale, _moveDuration / 2)
                    .SetEase(Ease.OutQuad));
                seq.Join(transform.DOScaleZ(_baseScale, _moveDuration / 2)
                    .SetEase(Ease.OutQuad));
            }
            else
            {
                seq.Append(transform.DOScaleZ(_scalePowerAnimation, _moveDuration / 2)
                    .SetEase(Ease.OutQuad));
                seq.Join(transform.DOScaleY(_scalePowerAnimation, _moveDuration / 2)
                    .SetEase(Ease.OutQuad));
                seq.Join(transform.DOScaleX(_baseScale - (_scalePowerAnimation - _baseScale), _moveDuration / 2)
                    .SetEase(Ease.OutQuad));
                
                seq.Append(transform.DOScaleZ(_baseScale, _moveDuration / 2)
                    .SetEase(Ease.InQuad));
                seq.Join(transform.DOScaleY(_baseScale, _moveDuration / 2)
                    .SetEase(Ease.OutQuad));
                seq.Join(transform.DOScaleX(_baseScale, _moveDuration / 2)
                    .SetEase(Ease.OutQuad));
            }
        }
        
        private void FinishMove()
        {
            _isMoving = false;
        }
        

        #endregion

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}

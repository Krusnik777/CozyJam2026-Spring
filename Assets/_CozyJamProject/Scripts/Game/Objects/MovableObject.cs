using System;
using CozySpringJam.Game.GameCycle;
using CozySpringJam.Game.Services;
using CozySpringJam.Game.SO;
using DG.Tweening;
using R3;
using UnityEngine;

namespace CozySpringJam.Game.Objects
{
    public class MovableObject : MonoBehaviour, IInteractable, IDisposable
    {
        [Header("References")]
        [SerializeField] private InteractZone _upTrigger;
        [SerializeField] private InteractZone _rightTrigger;
        [SerializeField] private InteractZone _downTrigger;
        [SerializeField] private InteractZone _leftTrigger;

        [Header("Settings")]
        [SerializeField] private float _moveDuration;
        [SerializeField] private float _scalePowerAnimation;

        public Subject<Unit> OnGridPositionChange = new();

        private float _baseScale;
        private Vector3 _direction = Vector3.zero;
        private bool _isMoving = false;

        private MovableObjectData _bindedMovableObjectData;

        private bool _isFailedMoving = false;

        private CompositeDisposable _disposables = new();

        public void Interact()
        {
            if (_isMoving) return;

            if (TryMoveToDirection())
                Move();
            else
                FailedMove();
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }

        public MovableObjectData CreateAndBindData()
        {
            _bindedMovableObjectData = new MovableObjectData
            {
                MovableObject = this,
                Position = new Vector2Int((int)transform.localPosition.x, (int)transform.localPosition.z)
            };
            
            return _bindedMovableObjectData;
        }

        private void Start()
        {
            _disposables.Add(_leftTrigger.OnEnter.Subscribe(_ =>
            {
                ChangeDirectionMove(new Vector2(2, 0));
                Interact();
                ParticleService.Instance.PlayParticle(ParticleType.BoxDust, _leftTrigger.transform.position + new Vector3(2, -0.35f, 0), Quaternion.identity);
            }));
            _disposables.Add(_rightTrigger.OnEnter.Subscribe(_ =>
            {
                ChangeDirectionMove(new Vector2(-2, 0));
                Interact();
                ParticleService.Instance.PlayParticle(ParticleType.BoxDust, _rightTrigger.transform.position + new Vector3(-2, -0.35f, 0), Quaternion.identity);
            }));
            _disposables.Add(_upTrigger.OnEnter.Subscribe(_ =>
            {
                ChangeDirectionMove(new Vector2(0, -2));
                Interact();
                ParticleService.Instance.PlayParticle(ParticleType.BoxDust, _upTrigger.transform.position + new Vector3(0, -0.35f, -2), Quaternion.identity);
            }));
            _disposables.Add(_downTrigger.OnEnter.Subscribe(_ =>
            {
                ChangeDirectionMove(new Vector2(0, 2));
                Interact();
                ParticleService.Instance.PlayParticle(ParticleType.BoxDust, _downTrigger.transform.position + new Vector3(0, -0.35f, 2), Quaternion.identity);
            }));

            _baseScale = transform.localScale.x;
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void UpdateGridPosition()
        {
            if (_bindedMovableObjectData == null) return;
            _bindedMovableObjectData.Position = new Vector2Int((int) transform.localPosition.x, (int) transform.localPosition.z); 

            OnGridPositionChange.OnNext(Unit.Default);
        }

        private void ChangeDirectionMove(Vector2 direction)
        {
            _direction = new Vector3(direction.x, 0, direction.y);
        }

        private bool TryMoveToDirection()
        {
            if (ShootRay(_direction, 1.5f, out RaycastHit hit))
            {
                return false;
            }

            return true;
        }

        private bool ShootRay(Vector3 direction, float distance, out RaycastHit hit)
        {
            var origin = transform.position + Vector3.up;
            Ray ray = new Ray(origin, direction.normalized);

            Debug.DrawRay(origin, direction.normalized * distance, Color.red, 1f);

            int layerMask = ~(1 << 7);

            return Physics.Raycast(ray, out hit, distance, layerMask);
        }

        #region Move
        private void Move()
        {
            _isMoving = true;
            Vector3 targetPosition = transform.position + _direction;
            SoundService.Instance.PlayMoveObject();
            transform.DOMove(targetPosition, _moveDuration).SetEase(Ease.InFlash).OnComplete(FinishMove);
            MoveAnimation();
        }

        private void FailedMove()
        {
            if(_isFailedMoving) return;
            _isFailedMoving = true;
            FailedMoveAnimation();
            SoundService.Instance.PlaySadMeow();
        }

        private void MoveAnimation()
        {
            var seq = DOTween.Sequence().SetLink(gameObject);
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

        private void FailedMoveAnimation()
        {
            var seq = DOTween.Sequence().SetLink(gameObject);
            if (_direction.x != 0)
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
            else
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
            seq.OnComplete((() => _isFailedMoving = false));
        }

        private void FinishMove()
        {
            _isMoving = false;
            transform.localPosition = new Vector3(Mathf.Round(transform.localPosition.x), transform.localPosition.y, Mathf.Round(transform.localPosition.z));
            UpdateGridPosition();
        }

        #endregion
    }
}

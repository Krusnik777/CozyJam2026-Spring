using DG.Tweening;
using UnityEngine;

namespace CozySpringJam.UI
{
    public class MovableRectView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rect;
        [SerializeField] private Vector2 _hiddenPosition;
        [SerializeField] private Vector2 _shownPosition;
        [SerializeField] private float _duration = 0.5f;

        public Vector2 DefaultPosition => _shownPosition;

        public bool IsActive { get; private set; }

        private Tween _currentTween;

        public void Show()
        {
            _currentTween?.Kill();

            IsActive = true;

            _currentTween = _rect
                .DOAnchorPos(_shownPosition, _duration)
                .SetEase(Ease.OutBack)
                .SetLink(gameObject)
                .OnComplete(OnShowCompleted);
        }

        public void Hide()
        {
            _currentTween?.Kill();

            IsActive = true;

            OnHide();

            _currentTween = _rect
                .DOAnchorPos(_hiddenPosition, _duration)
                .SetEase(Ease.InBack)
                .SetLink(gameObject)
                .OnComplete(() => IsActive = false);
        }

        protected virtual void OnShowCompleted()
        {
            IsActive = false;
        }
        protected virtual void OnHide() { }
    }
}

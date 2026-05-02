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
        
        private Tween _currentTween;

        public void Show()
        {
            _currentTween?.Kill();

            _currentTween = _rect
                .DOAnchorPos(_shownPosition, _duration)
                .SetEase(Ease.OutBack)
                .SetLink(gameObject);
        }

        public void Hide()
        {
            _currentTween?.Kill();

            _currentTween = _rect
                .DOAnchorPos(_hiddenPosition, _duration)
                .SetEase(Ease.InBack)
                .SetLink(gameObject);
        }
    }
}

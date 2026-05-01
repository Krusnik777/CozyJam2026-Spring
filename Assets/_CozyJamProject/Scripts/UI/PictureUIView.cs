using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CozySpringJam.UI
{
    public class PictureUIView : MonoBehaviour
    {
        [field: SerializeField] public Image PictureImage { get; private set; }
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
                .SetEase(Ease.OutBack);
        }

        public void Hide()
        {
            _currentTween?.Kill();

            _currentTween = _rect
                .DOAnchorPos(_hiddenPosition, _duration)
                .SetEase(Ease.InBack);
        }
    }
}
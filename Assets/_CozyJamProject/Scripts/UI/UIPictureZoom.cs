using UnityEngine;
using DG.Tweening;

namespace CozySpringJam.UI
{
    public class UIPictureZoom : MonoBehaviour
    {
        [SerializeField] private RectTransform m_targetRect;
        [SerializeField] private Vector2 m_targetPosition = Vector2.zero;
        [SerializeField] private Vector2 m_targetSize = new Vector2(1000f, 1000f);
        [SerializeField] private float m_duration = 0.5f;

        public bool IsZoomed { get; private set; }

        private bool _isZooming;
        private Sequence _animation;

        private Vector2 _startPosition;
        private Vector3 _startRotation;
        private Vector2 _startSize = new Vector2(400f, 400f);

        public void Zoom(Vector2 startPosition, Vector3 startRotation, Vector2 startSize)
        {
            if (_isZooming) return;

            _startPosition = startPosition;
            _startRotation = startRotation;
            _startSize = startSize;

            ToggleAnimation(true);
        }

        public void Unzoom()
        {
            if (_isZooming) return;

            ToggleAnimation(false);
        }

        private void ToggleAnimation(bool isZoom)
        {
            _animation?.Kill();

            _animation = DOTween.Sequence();
            _animation.SetLink(gameObject);

            _isZooming = true;

            var targetPosition = isZoom ? m_targetPosition : _startPosition;
            var targetRotation = isZoom ? Vector3.zero : _startRotation;
            var targetSize = isZoom ? m_targetSize : _startSize;

            _animation.Append(m_targetRect.DOAnchorPos(targetPosition, m_duration).SetEase(Ease.InSine));
            _animation.Join(m_targetRect.DOSizeDelta(targetSize, m_duration).SetEase(Ease.InSine));
            _animation.Join(m_targetRect.DORotate(targetRotation, m_duration).SetEase(Ease.Linear));

            _animation.OnComplete(() => _isZooming = false);

            IsZoomed = isZoom;
        }
    }
}

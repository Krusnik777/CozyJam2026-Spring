using System;
using DG.Tweening;
using UnityEngine;

namespace CozySpringJam.UI
{
    public class PictureUIView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rect;
        [SerializeField] private Vector2 _hiddenPosition;
        [SerializeField] private Vector2 _shownPosition;
        [SerializeField] private float _duration = 0.5f;
        private Tween _currentTween;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Show();
            }
            if (Input.GetMouseButtonDown(1))
            {
                Hide();
            }
        }
        
        [ContextMenu("Show")]
        public void Show()
        {
            _currentTween?.Kill();

            _currentTween = _rect
                .DOAnchorPos(_shownPosition, _duration)
                .SetEase(Ease.OutBack);
        }
        [ContextMenu("Hide")]
        public void Hide()
        {
            _currentTween?.Kill();

            _currentTween = _rect
                .DOAnchorPos(_hiddenPosition, _duration)
                .SetEase(Ease.InBack);
        }
    }
}
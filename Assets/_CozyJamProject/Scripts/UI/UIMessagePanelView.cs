using TMPro;
using UnityEngine;
using DG.Tweening;

namespace CozySpringJam.UI
{
    public class UIMessagePanelView : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_messageText;

        private Tween _showTween;
        private Tween _hideTween;

        private Color _currentColor;

        public void SetMessage(string message, Color color)
        {
            m_messageText.text = message;
            m_messageText.color = color;

            _currentColor = color;
        }

        public void PlayShow(float duration, System.Action onEnd)
        {
            _showTween?.Kill();
            _hideTween?.Kill();
            
            var startColor = _currentColor;
            startColor.a = 0;
            m_messageText.color = startColor;

            _showTween = m_messageText.DOColor(_currentColor, duration).SetEase(Ease.Linear).OnComplete(() => onEnd?.Invoke());
        }

        public void PlayHide(float duration)
        {
            _hideTween?.Kill();

            var targetColor = _currentColor;
            targetColor.a = 0;
            
            _hideTween = m_messageText.DOColor(targetColor, duration).SetEase(Ease.Linear).OnComplete(() => gameObject.SetActive(false));
        }
    }
}
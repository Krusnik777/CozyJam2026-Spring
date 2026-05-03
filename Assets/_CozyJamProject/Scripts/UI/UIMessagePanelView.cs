using TMPro;
using UnityEngine;
using DG.Tweening;

namespace CozySpringJam.UI
{
    public class UIMessagePanelView : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_messageText;

        private Sequence _showTween;
        private Tween _hideTween;

        private Color _currentColor;

        public void SetMessage(string message, Color color)
        {
            m_messageText.text = message;
            m_messageText.color = color;

            _currentColor = color;
        }

        public void PlayShow(float duration, float delay, System.Action onEnd)
        {
            _showTween?.Kill();
            _hideTween?.Kill();
            
            var startColor = _currentColor;
            startColor.a = 0;
            m_messageText.color = startColor;

            _showTween = DOTween.Sequence();
            _showTween.SetLink(gameObject);

            if (delay > 0f) _showTween.AppendInterval(delay);

            _showTween.Append(m_messageText.DOColor(_currentColor, duration).SetEase(Ease.Linear));

            _showTween.OnComplete(() => onEnd?.Invoke());
        }

        public void PlayHide(float duration)
        {
            _hideTween?.Kill();

            var targetColor = _currentColor;
            targetColor.a = 0;
            
            _hideTween = m_messageText.DOColor(targetColor, duration).SetEase(Ease.Linear).OnComplete(() => gameObject.SetActive(false));
            _hideTween.SetLink(gameObject);
        }
    }
}
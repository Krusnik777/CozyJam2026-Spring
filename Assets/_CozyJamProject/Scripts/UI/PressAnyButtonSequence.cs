using System.Collections;
using TMPro;
using UnityEngine;
using CozySpringJam.Game.Services;

namespace CozySpringJam.UI
{
    public class PressAnyButtonSequence : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_commandText;
        [SerializeField] private TMP_Text m_titleText;

        private GameInputService _gameInputService;
        private bool _buttonPressed;

        public void Bind(GameInputService gameInputService) => _gameInputService = gameInputService;

        public void StartSequence(float delay, System.Action onEnd)
        {
            if (_gameInputService == null)
            {
                onEnd?.Invoke();

                return;
            }

            _buttonPressed = false;

            m_commandText.text = "Press Any Button";

            StartCoroutine(WaitForButtonPressRoutine(delay, onEnd));
        }

        private void OnAnyButton()
        {
            _buttonPressed = true;
        }

        private void OnDestroy()
        {
            if (_gameInputService != null) _gameInputService.ClearReactionForAnyButtonPress();
        }

        private IEnumerator WaitForButtonPressRoutine(float delay, System.Action onEnd)
        {
            yield return new WaitForSeconds(delay);

            var titleDefaultColor = m_titleText.color;
            var titleNoColor = titleDefaultColor;
            titleNoColor.a = 0;

            m_titleText.gameObject.SetActive(true);

            yield return StartCoroutine(ChangeTextColorRoutine(m_titleText, titleNoColor, titleDefaultColor, 1f));

            yield return new WaitForSeconds(delay);

            var defaultColor = m_commandText.color;
            var noColor = m_commandText.color;
            noColor.a = 0;

            m_commandText.gameObject.SetActive(true);

            yield return StartCoroutine(ChangeTextColorRoutine(m_commandText, noColor, defaultColor, 1f));

            _gameInputService.SetReactionForAnyButtonPress(OnAnyButton);

            yield return new WaitUntil(() => _buttonPressed);

            yield return StartCoroutine(ChangeTextsColorRoutine(titleDefaultColor, defaultColor, titleNoColor, noColor, 1f));

            m_commandText.gameObject.SetActive(false);
            m_titleText.gameObject.SetActive(false);

            onEnd?.Invoke();
        }

        private IEnumerator ChangeTextColorRoutine(TMP_Text text, Color startColor, Color targetColor, float duration)
        {  float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = elapsed / duration;
                text.color = Color.Lerp(startColor, targetColor, t);

                yield return null;
            }

            text.color = targetColor;
        }

        private IEnumerator ChangeTextsColorRoutine(Color titleDefaultColor, Color commandDefaultColor, Color titleTargetColor, Color commandTargetColor, float duration)
        {  
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = elapsed / duration;
                m_commandText.color = Color.Lerp(commandDefaultColor, commandTargetColor, t);
                m_titleText.color = Color.Lerp(titleDefaultColor, titleTargetColor, t);

                yield return null;
            }

            m_commandText.color = commandTargetColor;
            m_titleText.color = titleTargetColor;
        }
    }
}

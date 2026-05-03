using System.Collections;
using TMPro;
using UnityEngine;
using R3;

namespace CozySpringJam.UI
{
    public class PressAnyButtonSequence : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_commandText;
        [SerializeField] private TMP_Text m_titleText;

        //public Observable<bool> PressedButton => _pressedButton;
        //private ReactiveProperty<bool> _pressedButton;

        System.IDisposable _disposable;

        public void StartSequence(float delay, System.Action onEnd)
        {
            _disposable = ControlDevice.OnControlDeviceChanged.Subscribe(device =>
            {
                if (device is UnityEngine.InputSystem.Gamepad)
                {
                    m_commandText.text = "Press Start Button";
                }
                else
                {
                    m_commandText.text = "Press Space";
                    //m_commandText.text = "Press Enter or Space";
                }
            });

            StartCoroutine(WaitForButtonPressRoutine(delay, onEnd));
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
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

            yield return new WaitUntil(() =>
            {
                return Input.GetButtonDown("Submit");
            });

            //yield return StartCoroutine(ChangeTextColorRoutine(m_commandText, defaultColor, noColor, 1f));
            yield return StartCoroutine(ChangeTextsColorRoutine(titleDefaultColor, defaultColor, titleNoColor, noColor, 1f));

            m_commandText.gameObject.SetActive(false);
            m_titleText.gameObject.SetActive(false);

            //_pressedButton.Value = true;

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

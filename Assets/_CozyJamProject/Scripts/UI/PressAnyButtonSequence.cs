using System.Collections;
using TMPro;
using UnityEngine;
using R3;

namespace CozySpringJam.UI
{
    public class PressAnyButtonSequence : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_commandText;

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
                    m_commandText.text = "Press Enter or Space";
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

            var defaultColor = m_commandText.color;
            var noColor = m_commandText.color;
            noColor.a = 0;

            m_commandText.gameObject.SetActive(true);

            yield return StartCoroutine(ChangeTextColorRoutine(noColor, defaultColor, 1f));

            yield return new WaitUntil(() =>
            {
                return Input.GetButtonDown("Submit");
            });

            yield return StartCoroutine(ChangeTextColorRoutine(defaultColor, noColor, 1f));

            m_commandText.gameObject.SetActive(false);

            //_pressedButton.Value = true;

            onEnd?.Invoke();
        }

        private IEnumerator ChangeTextColorRoutine(Color startColor, Color targetColor, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = elapsed / duration;
                m_commandText.color = Color.Lerp(startColor, targetColor, t);

                yield return null;
            }

            m_commandText.color = targetColor;
        }
    }
}

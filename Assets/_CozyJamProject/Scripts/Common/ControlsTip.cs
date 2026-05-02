using TMPro;
using UnityEngine;
using R3;

namespace CozySpringJam
{
    public class ControlsTip : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_text;
        [SerializeField] private string m_keyboardTip;
        [SerializeField] private string m_gamepadTip;

        private System.IDisposable disposable;

        public void Subscribe()
        {
            disposable = ControlDevice.OnControlDeviceChanged.Subscribe(ChangeTextByDevice);
        }

        public void Unsubscribe()
        {
            disposable?.Dispose();
        }

        private void ChangeTextByDevice(UnityEngine.InputSystem.InputDevice device)
        {
            if (device is UnityEngine.InputSystem.Gamepad)
            {
                m_text.text = m_gamepadTip;
            }
            else
            {
                m_text.text = m_keyboardTip;
            }
        }

    }
}

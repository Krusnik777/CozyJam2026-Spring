using TMPro;
using UnityEngine;
using R3;
using UnityEngine.UI;

namespace CozySpringJam
{
    public class ControlsTip : MonoBehaviour
    {
        [Header("Text")]
        [SerializeField] private TMP_Text m_text;
        [SerializeField] private string m_keyboardTip;
        [SerializeField] private string m_gamepadTip;
        [Header("Image")]
        [SerializeField] private Image m_image;
        [SerializeField] private Sprite m_keyboardButtonImage;
        [SerializeField] private Sprite m_gamepadButtonImage;

        private System.IDisposable disposable;

        public void Subscribe()
        {
            disposable = ControlDevice.OnControlDeviceChanged.Subscribe(ChangeTipByDevice);
        }

        public void Unsubscribe()
        {
            disposable?.Dispose();
        }

        private void ChangeTipByDevice(UnityEngine.InputSystem.InputDevice device)
        {
            if (device is UnityEngine.InputSystem.Gamepad)
            {
                m_text.text = m_gamepadTip;
                m_image.sprite = m_gamepadButtonImage;
            }
            else
            {
                m_text.text = m_keyboardTip;
                m_image.sprite = m_keyboardButtonImage;
            }
        }

    }
}

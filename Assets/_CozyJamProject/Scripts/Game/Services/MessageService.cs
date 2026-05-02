using CozySpringJam.UI;
using DG.Tweening;
using UnityEngine;

namespace CozySpringJam.Game.Services
{
    public class MessageService
    {
        private UIMessagePanelView _messagePanel;
        private Tween _lifeTween;

        public MessageService(UIMessagePanelView messagePanel)
        {
            _messagePanel = messagePanel;
            _messagePanel.gameObject.SetActive(false);
        }

        public void ShowMessage(MessageData data)
        {
            if (data.Force)
                _lifeTween?.Kill();
            else
                _lifeTween?.Kill(true);

            _messagePanel.SetMessage(data.Text, data.Color);
            _messagePanel.gameObject.SetActive(true);

            _messagePanel.PlayShow(data.ShowDuration, () =>
            {
                _lifeTween = DOVirtual.DelayedCall(data.StayDuration, () => _messagePanel.PlayHide(data.HideDuration)).SetLink(_messagePanel.gameObject);
            });
        }
    }

    public class MessageData
    {
        public readonly string ID;
        public readonly string Text;
        public readonly float StayDuration;
        public readonly float ShowDuration;
        public readonly float HideDuration;
        public readonly bool Force;
        public readonly Color Color;

        public MessageData(string id, string text, float stayDuration = 2f, float showDuration = 1f, float hideDuration = 1f, bool force = false, Color? color = null)
        {
            ID = id;
            Text = text;
            StayDuration = stayDuration;
            ShowDuration = showDuration;
            HideDuration = hideDuration;
            Force = force;
            Color = color ?? Color.white;
        }

        public MessageData(MessageContainer messageContainer)
        {
            ID = messageContainer.ID;
            Text = messageContainer.Text;
            StayDuration = messageContainer.StayDuration;
            ShowDuration = messageContainer.ShowDuration;
            HideDuration = messageContainer.HideDuration;
            Force = messageContainer.Force;
            Color = messageContainer.Color;
        }
    }
}

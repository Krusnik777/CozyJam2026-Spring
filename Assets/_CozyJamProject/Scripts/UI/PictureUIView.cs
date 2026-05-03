using UnityEngine;
using UnityEngine.UI;

namespace CozySpringJam.UI
{
    public class PictureUIView : MovableRectView
    {
        [field: SerializeField] public Image PictureImage { get; private set; }
        [SerializeField] private GameObject _buttonPrompt;

        protected override void OnShowCompleted()
        {
            base.OnShowCompleted();

            _buttonPrompt.SetActive(true);
        }

        protected override void OnHide()
        {
            base.OnHide();

            _buttonPrompt.SetActive(false);
        }
    }
}
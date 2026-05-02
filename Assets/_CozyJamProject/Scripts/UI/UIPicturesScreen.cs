using System;
using CozySpringJam.Game.GameCycle;
using R3;
using UnityEngine;

namespace CozySpringJam.UI
{
    public class UIPicturesScreen : MonoBehaviour, IDisposable
    {
        [SerializeField] private PictureUIView m_upperPictureView;
        [SerializeField] private PictureUIView m_lowerPictureView;
        [SerializeField] private MovableRectView m_tipMovableRectView;
        [SerializeField] private ControlsTip m_controlsTip;

        private CompositeDisposable _disposables;

        public void Dispose()
        {
            _disposables?.Dispose();
            m_controlsTip.Unsubscribe();
        }

        public void Setup(IUIScreenInfluencer<PicturesScreenSettings, Unit> screenInfluencer)
        {
            _disposables = new();

            screenInfluencer.ShowSignal.Subscribe(Show).AddTo(_disposables);     
            screenInfluencer.HideSignal.Subscribe(_ => Hide()).AddTo(_disposables);

            m_controlsTip.Subscribe();
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void Show(PicturesScreenSettings picturesScreenSettings)
        {
            m_upperPictureView.PictureImage.sprite = picturesScreenSettings.UpperPicture;
            m_lowerPictureView.PictureImage.sprite = picturesScreenSettings.LowerPicture;

            m_upperPictureView.Show();
            m_lowerPictureView.Show();
            m_tipMovableRectView.Show();
        }

        private void Hide()
        {
            m_upperPictureView.Hide();
            m_lowerPictureView.Hide();
            m_tipMovableRectView.Hide();
        }
    }
}

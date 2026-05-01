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

        private CompositeDisposable _disposables;

        public void Dispose()
        {
            _disposables?.Dispose();
        }

        public void Setup(IUIScreenInfluencer<PicturesScreenSettings> screenInfluencer)
        {
            _disposables = new();

            screenInfluencer.ShowSignal.Subscribe(Show).AddTo(_disposables);     
            screenInfluencer.HideSignal.Subscribe(_ => Hide()).AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

        private void Show(PicturesScreenSettings picturesScreenSettings)
        {
            m_upperPictureView.PictureImage.sprite = picturesScreenSettings.UpperPicture;
            m_lowerPictureView.PictureImage.sprite = picturesScreenSettings.LowerPicture;

            m_upperPictureView.Show();
            m_lowerPictureView.Show();
        }

        private void Hide()
        {
            m_upperPictureView.Hide();
            m_lowerPictureView.Hide();
        }

    }
}

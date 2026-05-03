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
        [SerializeField] private UIPictureZoom m_upperPictureZoom;
        [SerializeField] private UIPictureZoom m_lowerPictureZoom;
        [SerializeField] private MovableRectView m_tipMovableRectView;
        [SerializeField] private ControlsTip[] m_controlsTips;

        private CompositeDisposable _disposables;

        public void Dispose()
        {
            _disposables?.Dispose();

            for (int i = 0; i < m_controlsTips.Length; i++) m_controlsTips[i].Unsubscribe();
        }

        public void Setup(IUIPicturesScreenInfluencer screenInfluencer)
        {
            _disposables = new();

            screenInfluencer.ShowSignal.Subscribe(Show).AddTo(_disposables);     
            screenInfluencer.HideSignal.Subscribe(_ => Hide()).AddTo(_disposables);

            screenInfluencer.UpperZoomSignal.Subscribe(actions => HandleZoom(actions, m_upperPictureZoom)).AddTo(_disposables);
            screenInfluencer.LowerZoomSignal.Subscribe(actions => HandleZoom(actions, m_lowerPictureZoom)).AddTo(_disposables);

            for (int i = 0; i < m_controlsTips.Length; i++) m_controlsTips[i].Subscribe();
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

        private void HandleZoom((Action, Action) actions, UIPictureZoom pictureZoom)
        {
            Debug.Log("HERE 0");

            if (m_lowerPictureView.IsActive || m_upperPictureView.IsActive) return;

            var otherPictureZoom = pictureZoom == m_upperPictureZoom ? m_lowerPictureZoom : m_upperPictureZoom;

            if (otherPictureZoom.IsZoomed) return;

            if (pictureZoom.IsZoomed)
            {
                pictureZoom.Unzoom();

                actions.Item2?.Invoke();
            }
            else
            {
                var pictureView = pictureZoom == m_upperPictureZoom ? m_upperPictureView : m_lowerPictureView;
                var rectTransform = pictureView.transform as RectTransform;

                pictureZoom.Zoom(pictureView.DefaultPosition, rectTransform.rotation.eulerAngles, new Vector2(rectTransform.rect.width, rectTransform.rect.height));

                actions.Item1?.Invoke();
            }
        }
    }
}

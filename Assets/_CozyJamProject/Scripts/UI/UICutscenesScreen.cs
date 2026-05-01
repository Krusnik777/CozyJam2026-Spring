using System;
using CozySpringJam.Game.SO;
using DG.Tweening;
using R3;
using UnityEngine;

namespace CozySpringJam.UI
{
    public class UICutscenesScreen : MonoBehaviour, IDisposable
    {
        [SerializeField] private RectTransform[] m_cutsceneBorders;

        private CompositeDisposable _disposables;

        public void Dispose()
        {
            _disposables?.Dispose();
        }

        public void Setup(IUIScreenInfluencer<CutscenesScreenSettings, (CutscenesScreenSettings, System.Action)> screenInfluencer)
        {
            _disposables = new();

            screenInfluencer.ShowSignal.Subscribe(Show).AddTo(_disposables);
            screenInfluencer.HideSignal.Subscribe(Hide).AddTo(_disposables);
        }

        private void Start()
        {
            for (int i = 0; i < m_cutsceneBorders.Length; i++)
            {
                m_cutsceneBorders[i].localScale = new Vector3(1, 0, 1);
            }
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

        private void Show(CutscenesScreenSettings cutsceneScreenSettings)
        {
            for (int i = 0; i < m_cutsceneBorders.Length; i++)
            {
                var anim = m_cutsceneBorders[i].DOScaleY(1, cutsceneScreenSettings.BordersAppearTime).SetEase(Ease.InSine);
                anim.SetLink(gameObject);
            }
        }

        private void Hide((CutscenesScreenSettings, System.Action) cutsceneScreenSettingsData)
        {
            for (int i = 0; i < m_cutsceneBorders.Length; i++)
            {
                var anim = m_cutsceneBorders[i].DOScaleY(0, cutsceneScreenSettingsData.Item1.BordersDisappearTime).SetEase(Ease.InSine);
                if (i >= m_cutsceneBorders.Length - 1) anim.OnComplete(() =>
                {
                    cutsceneScreenSettingsData.Item2?.Invoke();
                });
                anim.SetLink(gameObject);
            }
        }
    }
}

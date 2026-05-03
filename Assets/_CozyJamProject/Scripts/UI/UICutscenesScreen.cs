using System;
using CozySpringJam.Game.SO;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace CozySpringJam.UI
{
    public class UICutscenesScreen : MonoBehaviour, IDisposable
    {
        [SerializeField] private RectTransform[] m_cutsceneBorders;
        [SerializeField] private Image m_fadeImage;
        [SerializeField] private PressAnyButtonSequence m_pressAnyButtonSequence;

        private IDisposable _waitingDisposable;
        private CompositeDisposable _disposables;

        public void Dispose()
        {
            _disposables?.Dispose();
        }

        public void Setup(IUICutscenesScreenInfluencer screenInfluencer)
        {
            _disposables = new();

            for (int i = 0; i < m_cutsceneBorders.Length; i++)
            {
                m_cutsceneBorders[i].localScale = new Vector3(1, 0, 1);
            }

            m_fadeImage.gameObject.SetActive(false);

            screenInfluencer.ShowSignal.Subscribe(Show).AddTo(_disposables);
            screenInfluencer.HideSignal.Subscribe(Hide).AddTo(_disposables);

            screenInfluencer.FadeInSignal.Subscribe(FadeIn).AddTo(_disposables);
            screenInfluencer.FadeOutSignal.Subscribe(FadeOut).AddTo(_disposables);

            screenInfluencer.PrepareFadeInSignal.Subscribe(PrepareFadeIn).AddTo(_disposables);

            screenInfluencer.InputSequenceSignal.Subscribe(StartInputSequence).AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _waitingDisposable?.Dispose();
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

        private void FadeIn(float duration)
        {
            if (duration <= 0f) return;

            m_fadeImage.color = Color.black;
            var color = Color.black;
            color.a = 0f;

            m_fadeImage.gameObject.SetActive(true);

            var anim = m_fadeImage.DOColor(color, duration).SetEase(Ease.InSine).OnComplete(() => m_fadeImage.gameObject.SetActive(false));
            anim.SetLink(gameObject);
        }

        private void FadeOut((float, System.Action) fadeoutParameters)
        {
            if (fadeoutParameters.Item1 <= 0f) return;

            var color = Color.black;
            color.a = 0f;
            m_fadeImage.color = color;

            m_fadeImage.gameObject.SetActive(true);

            var anim = m_fadeImage.DOColor(Color.black, fadeoutParameters.Item1).SetEase(Ease.InSine).OnComplete(() =>
            {
                //m_fadeImage.gameObject.SetActive(false);
                fadeoutParameters.Item2?.Invoke();
            });
            anim.SetLink(gameObject);
        }

        private void PrepareFadeIn((float, Action) waitingParameters)
        {
            m_fadeImage.color = Color.black;
            m_fadeImage.gameObject.SetActive(true);

            if (waitingParameters.Item1 <= 0)
            {
                waitingParameters.Item2?.Invoke();
                return;
            }

            _waitingDisposable = Observable.Interval(TimeSpan.FromSeconds(waitingParameters.Item1)).Subscribe(_ =>
            {
                _waitingDisposable?.Dispose();

                waitingParameters.Item2?.Invoke();
            });
        }

        private void StartInputSequence((float, Action) inputSequenceParameters)
        {
            m_pressAnyButtonSequence.StartSequence(inputSequenceParameters.Item1, inputSequenceParameters.Item2);
        }
    }
}

using System;
using CozySpringJam.Game.GameCycle;
using CozySpringJam.Game.SO;
using R3;

namespace CozySpringJam.Game.Services
{
    public class CutsceneService : IUIScreenInfluencer<CutscenesScreenSettings, (CutscenesScreenSettings, System.Action)>, IDisposable
    {
        public Subject<CutscenesScreenSettings> ShowSignal => _showSignal;
        private Subject<CutscenesScreenSettings> _showSignal = new();

        public Subject<(CutscenesScreenSettings, System.Action)> HideSignal => _hideSignal;
        private Subject<(CutscenesScreenSettings, System.Action)> _hideSignal = new();

        private CutscenesScreenSettings _cutsceneScreenSettings;

        private System.Action _repeatableAction;
        public IDisposable _disposable;

        public CutsceneService(CutscenesScreenSettings cutsceneScreenSettings)
        {
            _cutsceneScreenSettings = cutsceneScreenSettings;
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }

        public void PlayCutscene(CutsceneSettings cutsceneSettings, System.Action onEnd)
        {
            if (cutsceneSettings.Segments.Length == 0)
            {
                onEnd?.Invoke();
                return;
            }

            ShowSignal.OnNext(_cutsceneScreenSettings);

            _disposable?.Dispose();
            int currentSegment = 0;

            _repeatableAction = () =>
            {
                _disposable?.Dispose();

                currentSegment++;

                if (currentSegment > cutsceneSettings.Segments.Length - 1)
                {
                     HideSignal.OnNext((_cutsceneScreenSettings, onEnd));
                }
                else
                {
                    _disposable = ActivateCutsceneSegment(cutsceneSettings.Segments[currentSegment], _repeatableAction);
                }
            };

            _disposable = ActivateCutsceneSegment(cutsceneSettings.Segments[currentSegment], _repeatableAction);
        }

        private IDisposable ActivateCutsceneSegment(CutsceneSegment cutsceneSegment, System.Action onEnd)
        {
            if (cutsceneSegment.Duration <= 0)
            {
                onEnd?.Invoke();
                return null;
            }

            cutsceneSegment.Camera.SetActive(true);

            return Observable.Interval(TimeSpan.FromSeconds(cutsceneSegment.Duration)).Subscribe(_ =>
            {
                cutsceneSegment.Camera.SetActive(false);

                onEnd?.Invoke();
            });
        }
    }
}

using System;
using CozySpringJam.Game.GameCycle;
using CozySpringJam.Game.SO;
using R3;

namespace CozySpringJam.Game.Services
{
    public class CutsceneService : IUICutscenesScreenInfluencer, IDisposable
    {
        public Subject<CutscenesScreenSettings> ShowSignal { get; private set; } = new();
        public Subject<(CutscenesScreenSettings, Action)> HideSignal { get; private set; } = new();
        public Subject<(float, Action)> PrepareFadeInSignal { get; private set; } = new();
        public Subject<float> FadeInSignal { get; private set; } = new();
        public Subject<(float, Action)> FadeOutSignal { get; private set; } = new();

        private CutscenesScreenSettings _cutsceneScreenSettings;
        private MessageService _messageService;

        private Action _repeatableAction;
        public IDisposable _disposable;

        public CutsceneService(CutscenesScreenSettings cutsceneScreenSettings, MessageService messageService)
        {
            _cutsceneScreenSettings = cutsceneScreenSettings;
            _messageService = messageService;
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }

        public void PlayCutscene(CutsceneSettings cutsceneSettings, Action onEnd)
        {
            if (cutsceneSettings.Segments.Length == 0)
            {
                onEnd?.Invoke();
                return;
            }

            if (cutsceneSettings.ShowDelay > 0 && cutsceneSettings.FadeInDuration > 0)
            {
                PrepareFadeInSignal.OnNext((cutsceneSettings.ShowDelay, () => HandleCutscene(cutsceneSettings, onEnd)));
            }
            else
            {
                HandleCutscene(cutsceneSettings, onEnd);
            }
        }

        private void HandleCutscene(CutsceneSettings cutsceneSettings, Action onEnd)
        {
            ShowSignal.OnNext(_cutsceneScreenSettings);
            if (cutsceneSettings.FadeInDuration > 0f) FadeInSignal.OnNext(cutsceneSettings.FadeInDuration);

            _disposable?.Dispose();
            int currentSegment = 0;

            _repeatableAction = () =>
            {
                _disposable?.Dispose();

                var previousCamera = cutsceneSettings.Segments[currentSegment].Camera;

                currentSegment++;

                if (currentSegment > cutsceneSettings.Segments.Length - 1)
                {
                    if (cutsceneSettings.FadeOutDuration > 0f)
                    {
                        FadeOutSignal.OnNext((cutsceneSettings.FadeOutDuration, 
                                              () => HideSignal.OnNext((_cutsceneScreenSettings, onEnd))));
                    }
                    else
                    {
                        previousCamera.SetActive(false);

                        HideSignal.OnNext((_cutsceneScreenSettings, onEnd));
                    }
                }
                else
                {
                    previousCamera.SetActive(false);

                    _disposable = ActivateCutsceneSegment(cutsceneSettings.Segments[currentSegment], _repeatableAction);
                }
            };

            _disposable = ActivateCutsceneSegment(cutsceneSettings.Segments[currentSegment], _repeatableAction);
        }

        private IDisposable ActivateCutsceneSegment(CutsceneSegment cutsceneSegment, Action onEnd)
        {
            if (cutsceneSegment.Duration <= 0)
            {
                onEnd?.Invoke();
                return null;
            }

            cutsceneSegment.Camera.SetActive(true);
            if (cutsceneSegment.Message.ID != string.Empty) _messageService.ShowMessage(new(cutsceneSegment.Message), cutsceneSegment.Message.StartDelay);

            return Observable.Interval(TimeSpan.FromSeconds(cutsceneSegment.Duration)).Subscribe(_ =>
            {
                //cutsceneSegment.Camera.SetActive(false);

                onEnd?.Invoke();
            });
        }
    }
}

using System;
using System.Collections.Generic;
using CozySpringJam.Game.Services;
using DI;
using R3;

namespace CozySpringJam.Game.GameCycle
{
    public class GameCycleController : IDisposable, IUIScreenInfluencer<PicturesScreenSettings, Unit>
    {
        public Subject<Unit> OnFinish = new();

        private readonly GameCycleControllerView _view;
        private readonly MessageService _messageService;
        private readonly CutsceneService _cutsceneService;

        public Subject<PicturesScreenSettings> ShowSignal => _picturesShowSignal;
        private Subject<PicturesScreenSettings> _picturesShowSignal = new();

        public Subject<Unit> HideSignal => _picturesHideSignal;
        private Subject<Unit> _picturesHideSignal = new();

        private Dictionary<int, PuzzleZone> _puzzleZonesMap;

        private int _puzzleZoneGeneratedId = 0;

        private IDisposable _finalSegmentDisposable;
        private CompositeDisposable _puzzleZoneListenerDisposables;

        public GameCycleController(DIContainer sceneContainer, GameCycleControllerView view)
        {
            _view = view;
            _messageService = sceneContainer.Resolve<MessageService>();
            _cutsceneService = sceneContainer.Resolve<CutsceneService>();

            var soundService = sceneContainer.Resolve<SoundService>();
            var particleService = sceneContainer.Resolve<ParticleService>();

            _view.EventCollector.Bind(soundService, particleService);

            Init(soundService, particleService);

            soundService.PlayBackgroundMusic();
        }

        public void Dispose()
        {
            _view.PlayerInput.Dispose();
            _puzzleZoneListenerDisposables?.Dispose();
            _finalSegmentDisposable?.Dispose();
        }

        private void Init(SoundService soundService, ParticleService particleService)
        {
            _puzzleZoneListenerDisposables = new();
            _puzzleZonesMap = new();

            for (int i = 0; i < _view.PuzzleZones.Length; i++)
            {
                var id = GeneratePuzzleZoneId();
                var view = _view.PuzzleZones[i];
                view.ZoneCameraTransform.gameObject.SetActive(false); // just to be safe
                var puzzleZone = new PuzzleZone(id, view, soundService, particleService, _view.PlayerInput, _view.PlayerMovement);
                _puzzleZonesMap.Add(id, puzzleZone);

                puzzleZone.OnEnter.Subscribe(puzzleZone => HandlePuzzleZoneInteraction(puzzleZone, true)).AddTo(_puzzleZoneListenerDisposables);
                puzzleZone.OnExit.Subscribe(puzzleZone => HandlePuzzleZoneInteraction(puzzleZone, false)).AddTo(_puzzleZoneListenerDisposables);
                puzzleZone.OnFinish.Subscribe(puzzleZoneData => HandlePuzzleZoneFinish(puzzleZoneData, soundService)).AddTo(_puzzleZoneListenerDisposables);
            }

            if (_view.ShowEntryCutscene)
            {
                _view.PlayerAnimator.PlayWakeUpAnimation();
                _cutsceneService.PlayCutscene(_view.EntryCutsceneSettings, () => EnablePlayer());
            }
            else
            {
                EnablePlayer();
            }
        }

        private void EnablePlayer()
        {
            _view.PlayerInput.enabled = true;
            _view.PlayerMovement.enabled = true;
            _view.PlayerCameraTransform.gameObject.SetActive(true);
        }

        private void DisablePlayer()
        {
            _view.PlayerInput.enabled = false;
            _view.PlayerMovement.enabled = false;
            _view.PlayerMovement.Reset();
            //_view.PlayerCameraTransform.gameObject.SetActive(false);
        }

        private int GeneratePuzzleZoneId()
        {
            _puzzleZoneGeneratedId++;

            return _puzzleZoneGeneratedId;
        }

        private void HandlePuzzleZoneInteraction(PuzzleZoneView puzzleZoneView, bool isEnter)
        {
            puzzleZoneView.ZoneCameraTransform.gameObject.SetActive(isEnter);
            _view.PlayerCameraTransform.gameObject.SetActive(!isEnter);

            _view.PlayerMovement.SetIsometricMovement(!(isEnter & !puzzleZoneView.IsIsometric));

            if (isEnter) _picturesShowSignal.OnNext(puzzleZoneView.PicturesScreenSettings);
            else _picturesHideSignal.OnNext(Unit.Default);
        }

        private void HandlePuzzleZoneFinish((int, PuzzleZoneView) puzzleZoneData, SoundService soundService)
        {
            if (_puzzleZonesMap.ContainsKey(puzzleZoneData.Item1))
            {
                var id = puzzleZoneData.Item1;
                _puzzleZonesMap[id].Dispose();
                _puzzleZonesMap.Remove(id);
                
                soundService.PlaySuccessful();

                HandlePuzzleZoneInteraction(puzzleZoneData.Item2, false);

                DisablePlayer();

                _cutsceneService.PlayCutscene(puzzleZoneData.Item2.CutsceneAtEndSettings, () =>
                {
                    EnablePlayer();

                    if (_puzzleZonesMap.Count == 0) _messageService.ShowMessage(new("started_final_segment", "Now time to rest..."));
                });

                if (_puzzleZonesMap.Count == 0)
                {
                    _puzzleZoneListenerDisposables?.Dispose();

                    //_messageService.ShowMessage(new("started_final_segment", "Now time to rest..."));

                    _finalSegmentDisposable = _view.FinalEnterTrigger.OnEnter.Subscribe(_ => OnFinalSegmentEnd());
                }
            }
        }

        private void OnFinalSegmentEnd()
        {
            _finalSegmentDisposable?.Dispose();
            DisablePlayer();

            // Some player Animations

            _cutsceneService.PlayCutscene(_view.FinalCutsceneSettings, () =>
            {
                OnFinish.OnNext(Unit.Default);
            });
        }

        
    }
}

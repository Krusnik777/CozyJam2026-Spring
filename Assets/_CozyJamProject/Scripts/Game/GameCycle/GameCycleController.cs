using System;
using System.Collections.Generic;
using R3;

namespace CozySpringJam.Game.GameCycle
{
    public class GameCycleController : IDisposable, IUIScreenInfluencer<PicturesScreenSettings>
    {
        public Subject<Unit> OnFinish = new();
        
        private readonly GameCycleControllerView _view;

        private Dictionary<int, PuzzleZone> _puzzleZonesMap;

        private int _puzzleZoneGeneratedId = 0;

        private IDisposable _finalSegmentDisposable;
        private CompositeDisposable _puzzleZoneListenerDisposables;

        public Subject<PicturesScreenSettings> ShowSignal => _picturesShowSignal;
        private Subject<PicturesScreenSettings> _picturesShowSignal = new();

        public Subject<Unit> HideSignal => _picturesHideSignal;
        private  Subject<Unit> _picturesHideSignal = new();

        public GameCycleController(GameCycleControllerView view)
        {
            _view = view;

            Init();
        }

        public void Dispose()
        {
            _puzzleZoneListenerDisposables?.Dispose();
            _finalSegmentDisposable?.Dispose();
        }

        private void Init()
        {
            _puzzleZoneListenerDisposables = new();
            _puzzleZonesMap = new();

            InitPlayer();

            for (int i = 0; i < _view.PuzzleZones.Length; i++)
            {
                var id = GeneratePuzzleZoneId();
                var view = _view.PuzzleZones[i];
                view.ZoneCameraTransform.gameObject.SetActive(false); // just to be safe
                var puzzleZone = new PuzzleZone(id, view);
                _puzzleZonesMap.Add(id, puzzleZone);

                puzzleZone.OnEnter.Subscribe(puzzleZone => HandlePuzzleZoneInteraction(puzzleZone, true)).AddTo(_puzzleZoneListenerDisposables);
                puzzleZone.OnExit.Subscribe(puzzleZone => HandlePuzzleZoneInteraction(puzzleZone, false)).AddTo(_puzzleZoneListenerDisposables);
                puzzleZone.OnFinish.Subscribe(puzzleZoneData => HandlePuzzleZoneFinish(puzzleZoneData)).AddTo(_puzzleZoneListenerDisposables);
            }

            // Init Cutscene
        }

        private void InitPlayer()
        {
            _view.PlayerInput.enabled = true;
            _view.PlayerMovement.enabled = true;
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

            _view.PlayerMovement.SetIsometricMovement(!isEnter);

            if (isEnter) _picturesShowSignal.OnNext(puzzleZoneView.PicturesScreenSettings);
            else _picturesHideSignal.OnNext(Unit.Default);
        }

        private void HandlePuzzleZoneFinish((int, PuzzleZoneView) puzzleZoneData)
        {
            if (_puzzleZonesMap.ContainsKey(puzzleZoneData.Item1))
            {
                var id = puzzleZoneData.Item1;
                _puzzleZonesMap[id].Dispose();
                _puzzleZonesMap.Remove(id);

                HandlePuzzleZoneInteraction(puzzleZoneData.Item2, false);

                if (_puzzleZonesMap.Count == 0)
                {
                    _puzzleZoneListenerDisposables?.Dispose();

                    // Message "Now time to rest"

                    _finalSegmentDisposable = _view.FinalEnterTrigger.OnEnter.Subscribe(_ => OnFinalSegmentEnd());
                }
            }
        }

        private void OnFinalSegmentEnd()
        {
            // Play cutscene
            OnFinish.OnNext(Unit.Default);
        }
    }
}

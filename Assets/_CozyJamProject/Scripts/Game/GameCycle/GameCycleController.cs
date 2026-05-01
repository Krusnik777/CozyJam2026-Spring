using System;
using System.Collections.Generic;
using R3;

namespace CozySpringJam.Game.GameCycle
{
    public class GameCycleController : IDisposable
    {
        public Subject<Unit> OnFinish = new();
        
        private readonly GameCycleControllerView _view;

        private Dictionary<int, PuzzleZone> _puzzleZonesMap;

        private int _puzzleZoneGeneratedId = 0;

        private IDisposable _finalSegmentDisposable;
        private CompositeDisposable _puzzleZoneListenerDisposables;

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

            for (int i = 0; i < _view.PuzzleZones.Length; i++)
            {
                var id = GeneratePuzzleZoneId();
                var view = _view.PuzzleZones[i];
                var puzzleZone = new PuzzleZone(id, view);
                _puzzleZonesMap.Add(id, puzzleZone);

                puzzleZone.OnEnter.Subscribe(pz => HandlePuzzleZoneInteraction(pz, true)).AddTo(_puzzleZoneListenerDisposables);
                puzzleZone.OnExit.Subscribe(pz => HandlePuzzleZoneInteraction(pz, false)).AddTo(_puzzleZoneListenerDisposables);
                puzzleZone.OnFinish.Subscribe(HandlePuzzleZoneFinish).AddTo(_puzzleZoneListenerDisposables);
            }

            // Init Cutscene
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
        }

        private void HandlePuzzleZoneFinish(int puzzleZoneId)
        {
            if (_puzzleZonesMap.ContainsKey(puzzleZoneId))
            {
                _puzzleZonesMap[puzzleZoneId].Dispose();
                _puzzleZonesMap.Remove(puzzleZoneId);

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

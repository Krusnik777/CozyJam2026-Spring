using System;
using R3;

namespace CozySpringJam.Game.GameCycle
{
    public class PuzzleZone : IDisposable
    {
        private readonly int _uniqueId;
        private readonly PuzzleZoneView _view;

        public Subject<PuzzleZoneView> OnEnter = new();
        public Subject<PuzzleZoneView> OnExit = new();
        public Subject<int> OnFinish = new();

        private CompositeDisposable _disposables;

        public PuzzleZone(int id, PuzzleZoneView view)
        {
            _uniqueId = id;
            _view = view;

            Init();
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }

        private void Init()
        {
            _disposables = new();

            _view.EnterTrigger.OnEnter.Subscribe(_ => OnEnter.OnNext(_view)).AddTo(_disposables);
            _view.EnterTrigger.OnExit.Subscribe(_ => OnExit.OnNext(_view)).AddTo(_disposables);

            // Subscribe to movableObjects movements => CheckIfPuzzleSolved
        }

        private void CheckIfPuzzleSolved()
        {
            // if solved
            _view.HandleEnvironmentChange();

            OnFinish.OnNext(_uniqueId);
        }
    }
}

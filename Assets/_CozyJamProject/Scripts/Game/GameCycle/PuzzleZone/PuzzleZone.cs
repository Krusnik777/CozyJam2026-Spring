using System;
using System.Collections.Generic;
using CozySpringJam.Game.Objects;
using R3;
using UnityEngine;

namespace CozySpringJam.Game.GameCycle
{
    public class PuzzleZone : IDisposable
    {
        private readonly int _uniqueId;
        private readonly PuzzleZoneView _view;

        public Subject<PuzzleZoneView> OnEnter = new();
        public Subject<PuzzleZoneView> OnExit = new();
        public Subject<(int, PuzzleZoneView)> OnFinish = new();

        private List<MovableObjectData> _movableObjectsList;

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
            _movableObjectsList = new();

            _view.EnterTrigger.OnEnter.Subscribe(_ => OnEnter.OnNext(_view)).AddTo(_disposables);
            _view.EnterTrigger.OnExit.Subscribe(_ => OnExit.OnNext(_view)).AddTo(_disposables);

            for (int i = 0; i < _view.MovableObjects.Length; i++)
            {
                var movable = _view.MovableObjects[i];
                var data = movable.CreateAndBindData();
                _movableObjectsList.Add(data);

                movable.OnGridPositionChange.Subscribe(_ => CheckIfPuzzleSolved()).AddTo(_disposables);
            }

            // Subscribe to movableObjects movements => CheckIfPuzzleSolved
        }

        private void CheckIfPuzzleSolved()
        {
            bool isSolved = true;

            for (int i = 0; i < _view.PuzzleSolution.Length; i++)
            {
                var puzzle = _view.PuzzleSolution[i];

                for (int j = 0; j < _movableObjectsList.Count; j++)
                {
                    var createdMovableData = _movableObjectsList[j];

                    if (createdMovableData.MovableObject == puzzle.MovableObject)
                    {
                        isSolved = createdMovableData.Position == puzzle.Position;

                        break;
                    }
                }

                if (!isSolved) return;
            }

            for (int i = 0; i < _movableObjectsList.Count; i++) _movableObjectsList[i].MovableObject.Dispose();

            _view.HandleEnvironmentChange();

            OnFinish.OnNext((_uniqueId, _view));
        }

    }
}

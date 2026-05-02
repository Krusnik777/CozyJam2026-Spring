using System;
using System.Collections.Generic;
using CozySpringJam.Game.Player;
using CozySpringJam.Game.Services;
using R3;
using UnityEngine;

namespace CozySpringJam.Game.GameCycle
{
    public class PuzzleZone : IDisposable
    {
        private readonly int _uniqueId;
        private readonly PuzzleZoneView _view;
        private readonly PlayerAvatarInput _playerInput;
        private readonly CharacterController _playerCharacter;

        public Subject<PuzzleZoneView> OnEnter = new();
        public Subject<PuzzleZoneView> OnExit = new();
        public Subject<(int, PuzzleZoneView)> OnFinish = new();

        private List<MovableObjectData> _movableObjectsList;

        private IDisposable _resetListenerDisposable;
        private CompositeDisposable _disposables;

        public PuzzleZone(int id, PuzzleZoneView view, SoundService soundService, ParticleService particleService, PlayerAvatarInput playerInput, CharacterController playerCharacter)
        {
            _uniqueId = id;
            _view = view;
            _playerInput = playerInput;
            _playerCharacter = playerCharacter;

            Init(soundService, particleService);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            _resetListenerDisposable?.Dispose();
        }

        private void Init(SoundService soundService, ParticleService particleService)
        {
            _disposables = new();
            _movableObjectsList = new();

            _view.EnterTrigger.OnEnter.Subscribe(_ => HandleEnter()).AddTo(_disposables);
            _view.EnterTrigger.OnExit.Subscribe(_ => HandleExit()).AddTo(_disposables);

            for (int i = 0; i < _view.MovableObjects.Length; i++)
            {
                var movable = _view.MovableObjects[i];
                movable.Initialize(particleService, soundService);
                var data = movable.CreateAndBindData();
                _movableObjectsList.Add(data);

                movable.OnGridPositionChange.Subscribe(_ => CheckIfPuzzleSolved()).AddTo(_disposables);
            }
        }

        private void HandleEnter()
        {
            _resetListenerDisposable = _playerInput.OnResetButtonPressed.Subscribe(_ => ResetPuzzle());

            OnEnter.OnNext(_view);
        }

        private void HandleExit()
        {
            _resetListenerDisposable?.Dispose();

            OnExit.OnNext(_view);
        }

        private void ResetPuzzle()
        {
            _playerCharacter.enabled = false;

            _playerInput.transform.position = _view.PlayerTargetPositionOnReset.position;
            for (int i = 0; i < _view.MovableObjects.Length; i++)
            {
                var movable = _view.MovableObjects[i];
                movable.ResetPosition();
            }

            _playerCharacter.enabled = true;
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

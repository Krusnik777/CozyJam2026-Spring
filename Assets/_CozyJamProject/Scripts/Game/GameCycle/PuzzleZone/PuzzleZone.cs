using System;
using System.Collections.Generic;
using CozySpringJam.Game.Player;
using CozySpringJam.Game.Services;
using CozySpringJam.Game.SO;
using R3;
using UnityEngine;

namespace CozySpringJam.Game.GameCycle
{
    public class PuzzleZone : IDisposable
    {
        private readonly int _uniqueId;
        private readonly PuzzleZoneView _view;
        private readonly PlayerAvatarInput _playerInput;
        private readonly PlayerAvatarMovement _playerMovement;

        public Subject<PuzzleZoneView> OnEnter = new();
        public Subject<PuzzleZoneView> OnExit = new();
        public Subject<(int, PuzzleZoneView)> OnFinish = new();

        private List<MovableObjectData> _movableObjectsList;

        private IDisposable _resetListenerDisposable;
        private IDisposable _completeListenerDisposable;
        private CompositeDisposable _disposables;
        private ParticleService _particleService;
        private SoundService _soundService;
        
        public PuzzleZone(int id, PuzzleZoneView view,
                          SoundService soundService, ParticleService particleService,
                          PlayerAvatarInput playerInput, PlayerAvatarMovement playerMovement)
        {
            _uniqueId = id;
            _view = view;
            _playerInput = playerInput;
            _playerMovement = playerMovement;
            
            Init(soundService, particleService);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            _resetListenerDisposable?.Dispose();
            _completeListenerDisposable?.Dispose();
        }

        private void Init(SoundService soundService, ParticleService particleService)
        {
            _disposables = new();
            _movableObjectsList = new();
            _particleService = particleService;
            _soundService = soundService;
            
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

            for (int i = 0; i < _view.ServicesReceivers.Length; i++)
            {
                _view.ServicesReceivers[i].Bind(soundService, particleService);
            }
        }

        private void HandleEnter()
        {
            _resetListenerDisposable?.Dispose();
            _resetListenerDisposable = _playerInput.OnResetButtonPressed.Subscribe(_ => ResetPuzzle());
            
            _completeListenerDisposable?.Dispose();
            _completeListenerDisposable = _playerInput.OnCompleteButtonPressed.Subscribe(_ => CompletePuzzle());

            OnEnter.OnNext(_view);
        }

        private void HandleExit()
        {
            _resetListenerDisposable?.Dispose();

            OnExit.OnNext(_view);
        }

        private void ResetPuzzle()
        {
            _particleService.PlayParticle(ParticleType.TeleportDust, _playerMovement.transform.position, Quaternion.Euler(new Vector3(270,270,0)));
            _soundService.PlayPuff();
            _playerMovement.Teleport(_view.PlayerTargetPositionOnReset);
            _particleService.PlayParticle(ParticleType.TeleportDust, _view.PlayerTargetPositionOnReset.position, Quaternion.Euler(new Vector3(270,270,0)));
            for (int i = 0; i < _view.MovableObjects.Length; i++)
            {
                var movable = _view.MovableObjects[i];
                movable.ResetPosition();
            }
        }

        private void CompletePuzzle()
        {
            for (int i = 0; i < _view.PuzzleSolution.Length; i++)
            {
                var solution = _view.PuzzleSolution[i];

                for (int j = 0; j < _movableObjectsList.Count; j++)
                {
                    var movableData = _movableObjectsList[j];
                    
                    if (movableData.MovableObject == solution.MovableObject)
                    {
                        movableData.Position = solution.Position;
                        solution.MovableObject.transform.localPosition = new Vector3(solution.Position.x, 0f, solution.Position.y);
                        break;
                    }
                }
            }
            
            CheckIfPuzzleSolved();
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

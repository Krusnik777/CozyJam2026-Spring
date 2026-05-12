using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace CozySpringJam.Game.Services
{
    public class GameInputService : IDisposable
    {
        private struct PlayerInputActions
        {
            public Action OnInteract;
            public Action OnReset;
            public Action OnUpperZoom;
            public Action OnLowerZoom;
            public Action OnExitGame;
            public Action OnCompletePuzzle;
        }

        public Subject<Unit> OnExitGameButtonPressed { get; private set;} = new();

        public Subject<Unit> OnResetButtonPressed { get; private set;} = new();
        public Subject<Unit> OnCompleteButtonPressed { get; private set;} = new();

        public Subject<Unit> OnUpperPictureZoomButtonPressed { get; private set;} = new();
        public Subject<Unit> OnLowerPictureZoomButtonPressed { get; private set;} = new();

        public bool IsLimitedControls { get; set;} = false;

        private GameInput _gameInput;
        private PlayerInputActions _playerInputActionsMap;

        private bool _playerControlEnabled;

        private IDisposable _anyButtonPressListenerDisposable;

        public GameInputService()
        {
            _gameInput = new();
            _gameInput.Enable();

            _playerControlEnabled = false;

            _playerInputActionsMap = new();
            
            _gameInput.Player.Interact.performed += OnPlayerInteract;
            _gameInput.Player.Reset.performed += OnPlayerReset;
            _gameInput.Player.UpperZoom.performed += OnPlayerUpperZoom;
            _gameInput.Player.LowerZoom.performed += OnPlayerLowerZoom;
            _gameInput.Player.ExitGame.performed += OnPlayerExitGame;

            #if UNITY_EDITOR
            
            _gameInput.Player.CompletePuzzle.performed += OnPlayerCompletePuzzle;

            #endif
        }

        public Vector3 GetMovementInput(bool isInverse = true)
        {
            var input = _gameInput.Player.Move.ReadValue<Vector2>();
            if (isInverse) input *= -1f;

            return new Vector3(input.x, 0, input.y);
        }

        public void Dispose()
        {
            _anyButtonPressListenerDisposable?.Dispose();

            _gameInput.Player.Interact.performed -= OnPlayerInteract;
            _gameInput.Player.Reset.performed -= OnPlayerReset;
            _gameInput.Player.UpperZoom.performed -= OnPlayerUpperZoom;
            _gameInput.Player.LowerZoom.performed -= OnPlayerLowerZoom;
            _gameInput.Player.ExitGame.performed -= OnPlayerExitGame;

            _gameInput.Player.CompletePuzzle.performed -= OnPlayerCompletePuzzle;
        }

        public void ClearReactionForAnyButtonPress() => _anyButtonPressListenerDisposable?.Dispose();

        public void SetPlayerInputsActive(bool active) => _playerControlEnabled = active;

        public void SetReactionForAnyButtonPress(Action action)
        {
            _anyButtonPressListenerDisposable?.Dispose();

            _anyButtonPressListenerDisposable = InputSystem.onAnyButtonPress.Call(_ =>
            {
               _anyButtonPressListenerDisposable?.Dispose();

               action?.Invoke(); 
            });
        }

        public void SetOnPlayerInteract(Action action) => _playerInputActionsMap.OnInteract = action;

        private void OnPlayerInteract(InputAction.CallbackContext context)
        {
            if (IsLimitedControls || !_playerControlEnabled) return;

            _playerInputActionsMap.OnInteract?.Invoke();
        }

        private void OnPlayerReset(InputAction.CallbackContext context)
        {
            if (IsLimitedControls || !_playerControlEnabled) return;

            _playerInputActionsMap.OnReset?.Invoke();

            OnResetButtonPressed.OnNext(Unit.Default);
        }

        private void OnPlayerUpperZoom(InputAction.CallbackContext context)
        {
            if (!_playerControlEnabled) return;

            _playerInputActionsMap.OnUpperZoom?.Invoke();

            OnUpperPictureZoomButtonPressed.OnNext(Unit.Default);
        }

        private void OnPlayerLowerZoom(InputAction.CallbackContext context)
        {
            if (!_playerControlEnabled) return;

            _playerInputActionsMap.OnLowerZoom?.Invoke();

            OnLowerPictureZoomButtonPressed.OnNext(Unit.Default);
        }

        private void OnPlayerExitGame(InputAction.CallbackContext context)
        {
            if (!_playerControlEnabled) return;

            _playerInputActionsMap.OnExitGame?.Invoke();

            OnExitGameButtonPressed.OnNext(Unit.Default);
        }

        private void OnPlayerCompletePuzzle(InputAction.CallbackContext context)
        {
            if (IsLimitedControls || !_playerControlEnabled) return;

            _playerInputActionsMap.OnCompletePuzzle?.Invoke();

            OnCompleteButtonPressed?.OnNext(Unit.Default);
        }
    }
}

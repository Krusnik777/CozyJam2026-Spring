using System;
using UnityEngine.InputSystem;

namespace CozySpringJam.Game.Services
{
    public class InputDeviceDetectService : IDisposable
    {
        private InputDevice _currentDevice;

        public void Dispose()
        {
            InputSystem.onEvent -= OnDeviceChange;
        }

        public InputDeviceDetectService()
        {
            InputSystem.onEvent += OnDeviceChange;
        }

        private void OnDeviceChange(UnityEngine.InputSystem.LowLevel.InputEventPtr eventPtr, InputDevice device)
        {
            if (_currentDevice == device) return;

            _currentDevice = device;

            ControlDevice.OnControlDeviceChanged.OnNext(_currentDevice);
        }


    }
}

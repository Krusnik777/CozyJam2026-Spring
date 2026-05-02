using R3;
using UnityEngine.InputSystem;

namespace CozySpringJam
{
    public static class ControlDevice
    {
        public static Subject<InputDevice> OnControlDeviceChanged = new();
    }
}

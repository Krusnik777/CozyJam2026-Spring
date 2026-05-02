using UnityEngine;

namespace CozySpringJam.Game.Services
{
    [System.Serializable]
    public class MessageContainer
    {
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public string Text { get; private set; }
        [field: SerializeField] public float StartDelay { get; private set; } = 0f;
        [field: SerializeField] public float StayDuration { get; private set; } = 2f;
        [field: SerializeField] public float ShowDuration { get; private set; } = 1f;
        [field: SerializeField] public float HideDuration { get; private set; } = 1f;
        [field: SerializeField] public bool Force { get; private set; } = false;
        [field: SerializeField] public Color Color { get; private set; } = Color.white;
    }
}

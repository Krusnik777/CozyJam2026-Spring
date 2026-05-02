using CozySpringJam.Game.Services;
using UnityEngine;

namespace CozySpringJam.Game.GameCycle
{
    [System.Serializable]
    public class CutsceneSegment
    {
        [field: SerializeField] public GameObject Camera { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public MessageContainer Message { get; private set; }
    }

    [System.Serializable]
    public class CutsceneSettings
    {
        [field: SerializeField] public float ShowDelay { get; private set; } 
        [field: SerializeField] public CutsceneSegment[] Segments { get; private set; } 
        [field: SerializeField] public float FadeInDuration { get; private set; } = 0f;
        [field: SerializeField] public float FadeOutDuration { get; private set; } = 0f;
    }
}

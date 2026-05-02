using UnityEngine;

namespace CozySpringJam.Game.GameCycle
{
    [System.Serializable]
    public class CutsceneSegment
    {
        [field: SerializeField] public GameObject Camera { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
    }

    [System.Serializable]
    public class CutsceneSettings
    {
        [field: SerializeField] public CutsceneSegment[] Segments { get; private set; } 
        [field: SerializeField] public string Message { get; private set; }  
    }
}

using CozySpringJam.Game.Services;
using UnityEngine;

namespace CozySpringJam.Game.GameCycle
{
    [System.Serializable]
    public class CutsceneSegment
    {
        [field: SerializeField] public bool WaitingInput { get; protected set; }
        [field: SerializeField] public GameObject Camera { get; protected set; }
        [field: SerializeField] public float Duration { get; protected set; }
        [field: SerializeField] public MessageContainer Message { get; protected set; }
    }

    public class InputCutsceneSegment : CutsceneSegment
    {
        public System.Action OnInput;

        public InputCutsceneSegment(CutsceneSegment inputCutscene, System.Action onInput)
        {
            WaitingInput = true;
            Camera = inputCutscene.Camera;
            Duration = inputCutscene.Duration;
            Message = inputCutscene.Message;
            OnInput = onInput;
        }
    }

    [System.Serializable]
    public class CutsceneSettings
    {
        [field: SerializeField] public float ShowDelay { get; protected set; } 
        [field: SerializeField] public CutsceneSegment[] Segments { get; protected set; } 
        [field: SerializeField] public float FadeInDuration { get; protected set; } = 0f;
        [field: SerializeField] public float FadeOutDuration { get; protected set; } = 0f;
    }

    public class CutsceneWithInputsSettings : CutsceneSettings
    {
        public CutsceneWithInputsSettings(CutsceneSettings original, CutsceneSegment[] newSegments)
        {
            ShowDelay = original.ShowDelay;
            Segments = newSegments;
            FadeInDuration = original.FadeInDuration;
            FadeOutDuration = original.FadeOutDuration;
        }
    }
}

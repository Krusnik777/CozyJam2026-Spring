using CozySpringJam.Game.Objects;
using CozySpringJam.Game.Player;
using UnityEngine;

namespace CozySpringJam.Game.GameCycle
{
    public class GameCycleControllerView : MonoBehaviour
    {
        [field: Header("Player")]
        [field: SerializeField] public PlayerAvatarMovement PlayerMovement { get; private set; } 
        [field: SerializeField] public PlayerAvatarAnimator PlayerAnimator { get; private set; } 
        [field: SerializeField] public PlayerAvatarInput PlayerInput { get; private set; } 
        [field: SerializeField] public EventCollector EventCollector { get; private set; } 
        [field: Header("Camera")]
        [field: SerializeField] public Transform PlayerCameraTransform { get; private set; } 
        [field: Header("Start Segment")]
        [field: SerializeField] public CutsceneSettings EntryCutsceneSettings { get; private set; } 
        [field: Header("Puzzle Zones")]
        [field: SerializeField] public PuzzleZoneView[] PuzzleZones { get; private set; }  
        [field: Header("Final Segment")]
        [field: SerializeField] public EnterTrigger FinalEnterTrigger { get; private set; }
        [field: SerializeField] public GameObject FinalSceneHolder { get; private set; }
        [field: SerializeField] public CutsceneSettings FinalCutsceneSettings { get; private set; } 
        [field: Header("DEBUG")]
        [field: SerializeField] public bool ShowEntryCutscene { get; private set; } = true;
    }
}

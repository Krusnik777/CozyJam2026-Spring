using CozySpringJam.Game.Player;
using UnityEngine;

namespace CozySpringJam.Game.GameCycle
{
    public class GameCycleControllerView : MonoBehaviour
    {
        [field: Header("Player")]
        [field: SerializeField] public Transform PlayerTransform { get; private set; }  
        [field: SerializeField] public PlayerAvatarAnimator PlayerAnimator { get; private set; } 
        [field: SerializeField] public PlayerAvatarInput PlayerInput { get; private set; } 
        [field: SerializeField] public PlayerAvatarMovement PlayerMovement { get; private set; } 
        [field: Header("Camera")]
        [field: SerializeField] public Transform PlayerCameraTransform { get; private set; } 
        [field: Header("Puzzle Zones")]
        [field: SerializeField] public PuzzleZoneView[] PuzzleZones { get; private set; }  
        // First Cutscene
        [field: Header("Final Segment")]
        [field: SerializeField] public EnterTrigger FinalEnterTrigger { get; private set; }
        // Final Cutscene
    }
}

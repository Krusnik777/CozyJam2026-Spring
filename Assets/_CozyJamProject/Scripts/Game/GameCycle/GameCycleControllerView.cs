using CozySpringJam.Game.Player;
using UnityEngine;

namespace CozySpringJam.Game.GameCycle
{
    public class GameCycleControllerView : MonoBehaviour
    {
        [field: Header("Player")]
        [field: SerializeField] public PlayerAvatarAnimator PlayerAnimator { get; private set; } 
        [field: SerializeField] public PlayerAvatarInput PlayerInput { get; private set; } 
        [field: SerializeField] public PlayerAvatarMovement PlayerMovement { get; private set; } 
        //[field: SerializeField] public PuzzleZoneView[] PuzzleZones { get; private set; }
        // First Cutscene
        // Final Cutscene
    }
}

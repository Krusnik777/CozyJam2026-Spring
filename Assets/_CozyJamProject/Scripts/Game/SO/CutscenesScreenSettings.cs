using UnityEngine;

namespace CozySpringJam.Game.SO
{
    [CreateAssetMenu(fileName = "CutscenesScreenSettings", menuName = "Scriptable Objects/Cutscenes Screen Settings")]
    public class CutscenesScreenSettings : ScriptableObject
    {
        [field: SerializeField] public float BordersAppearTime { get; private set; } = 0.25f;
        [field: SerializeField] public float BordersDisappearTime { get; private set; } = 0.25f;
    }
}

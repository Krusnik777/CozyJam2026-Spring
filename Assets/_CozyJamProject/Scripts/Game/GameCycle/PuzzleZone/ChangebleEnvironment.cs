using UnityEngine;

namespace CozySpringJam.Game.GameCycle
{
    [System.Serializable]
    public class ChangebleEnvironment
    {
        [System.Serializable]
        public class TargetObject
        {
            public GameObject targetObject;
            public float duration;
        }

        public TargetObject[] objectsToEnable;
        public TargetObject[] objectsToDisable;
        // Maybe some materials also
    }
}

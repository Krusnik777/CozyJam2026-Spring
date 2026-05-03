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
        public ChangeableRenderer[] changeableRenderers;
        public DisableableRenderer[] disableableRenderers;
    }

    [System.Serializable]
    public class ChangeableRenderer
    {
        public MeshRenderer[] renderers;
        public int targetMaterialIndex = 1;
        public float alphaTargetValue = 0f;
        public float duration;
    }

    [System.Serializable]
    public class DisableableRenderer
    {
        public MeshRenderer[] renderers;
        public int targetMaterialIndex;
    }
}

using CozySpringJam.Game.Objects;
using DG.Tweening;
using UnityEngine;

namespace CozySpringJam.Game.GameCycle
{
    public class PuzzleZoneView : MonoBehaviour
    {
        [field: SerializeField] public bool IsIsometric { get; private set; } = true;
        [field: SerializeField] public EnterTrigger EnterTrigger { get; private set; }
        [field: SerializeField] public Transform ZoneCameraTransform { get; private set; }
        [field: SerializeField] public MovableObjectData[] PuzzleSolution { get; private set; }
        [field: SerializeField] public MovableObject[] MovableObjects { get; private set; }
        [field: SerializeField] public PicturesScreenSettings PicturesScreenSettings { get; private set; } 
        [field: SerializeField] public Transform PlayerTargetPositionOnReset { get; private set; } 
        [field: SerializeField] public CutsceneSettings CutsceneAtEndSettings { get; private set; } 
        [SerializeField] private ChangebleEnvironment m_changeableEnvironment;

        public void HandleEnvironmentChange(System.Action onEnd = null)
        {
            for (int i = 0; i < m_changeableEnvironment.objectsToEnable.Length; i++)
            {
                var target = m_changeableEnvironment.objectsToEnable[i].targetObject;
                target.SetActive(true);

                if (m_changeableEnvironment.objectsToEnable[i].duration <= 0) continue;

                var duration = m_changeableEnvironment.objectsToEnable[i].duration;

                if (duration > 0f)
                {
                    float baseScale = target.transform.localScale.x;
                    target.transform.localScale = Vector3.zero;
                    var anim = target.transform.DOScale(baseScale, duration).SetEase(Ease.OutBack).OnComplete(() => onEnd?.Invoke());
                    anim.SetLink(gameObject);
                }
            }

            for (int i = 0; i < m_changeableEnvironment.objectsToDisable.Length; i++)
            {
                var target = m_changeableEnvironment.objectsToDisable[i].targetObject;
                var duration = m_changeableEnvironment.objectsToDisable[i].duration;

                if (duration > 0f)
                {
                    var anim = target.transform.DOScale(Vector3.zero, duration).SetEase(Ease.OutBack).OnComplete(() => target.SetActive(false));
                    anim.SetLink(gameObject);
                }
                else
                {
                    target.SetActive(false);
                }
            }
        }
    }
}

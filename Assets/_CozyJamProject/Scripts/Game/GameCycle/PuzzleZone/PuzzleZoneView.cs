using System.Collections.Generic;
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
        [field: SerializeField] public SoundReceiver[] SoundReceivers { get; private set; }
        [field: SerializeField] public ParticleReceiver[] ParticleReceivers { get; private set; }
        
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
                    //ProcessCollidersRecursively(target.transform, true, true);

                    Vector3 baseScale = target.transform.localScale;
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
                    ProcessCollidersRecursively(target.transform, true, false);

                    var anim = target.transform.DOScale(Vector3.zero, duration).SetEase(Ease.OutBack).OnComplete(() => target.SetActive(false));
                    anim.SetLink(gameObject);
                }
                else
                {
                    target.SetActive(false);
                }
            }

            for (int i = 0; i < m_changeableEnvironment.changeableRenderers.Length; i++)
            {
                var target = m_changeableEnvironment.changeableRenderers[i];

                for (int j = 0; j < target.renderers.Length; j++)
                {
                    var renderer = target.renderers[j];

                    if (renderer.materials.Length == 0 || target.targetMaterialIndex > renderer.materials.Length) continue;

                    var material = renderer.materials[target.targetMaterialIndex];
                    var color = renderer.materials[target.targetMaterialIndex].color;
                    color.a = target.alphaTargetValue;

                    if (target.duration > 0)
                    {
                        var anim = DOTween.Sequence();
                        anim.SetLink(gameObject);

                        anim.Append(material.DOColor(color, target.duration));
                        anim.OnComplete(() =>
                        {
                            onEnd?.Invoke();
                            // Delete Material
                        });
                    }
                    else
                    {
                        renderer.materials[target.targetMaterialIndex].color = color;
                        // Delete Material
                    }
                }
            }

            for (int i = 0; i < m_changeableEnvironment.disableableRenderers.Length; i++)
            {
                var target = m_changeableEnvironment.disableableRenderers[i];

                for (int j = 0; j < target.renderers.Length; j++)
                {
                    var renderer = target.renderers[j];

                    if (target.targetMaterialIndex >= renderer.materials.Length)
                    {
                        Debug.LogWarning($"[PuzzleZoneView - ChangebleEnvironment] Index for target material out of bounds: {renderer.name} : {target.targetMaterialIndex} vs Length {renderer.materials.Length}");

                        continue;
                    }

                    List<Material> materialsList = new();

                    for (int k = 0; k < renderer.materials.Length; k++)
                    {
                        if (k == target.targetMaterialIndex) continue;

                        materialsList.Add(renderer.materials[k]);
                    }

                    renderer.materials = materialsList.ToArray();
                }
            }
        }

        private void ProcessCollidersRecursively(Transform parent, bool includeSelf, bool enable)
        {
            if (parent == null) return;

            if (includeSelf)
            {
                Collider[] colliders = parent.GetComponents<Collider>();
                foreach (Collider col in colliders)
                {
                    if (col != null)
                        col.enabled = enable;
                }
            }

            foreach (Transform child in parent)
            {
                ProcessCollidersRecursively(child, true, enable);
            }
        }

        /*private void HandleMaterialBlendForRenderer(float duration, Material targetMaterial, MeshRenderer renderer, int materialIndex = -1)
        {
            if (materialIndex <= 0)
            {
                var initialMaterial = new Material(renderer.material);
                var blendMaterial = new Material(renderer.material);
                //renderer.material = blendMaterial;

                var materialTween = DOVirtual.Float(0, 1, duration, (value) =>
                {
                    blendMaterial.Lerp(initialMaterial, targetMaterial, value);
                    renderer.material = blendMaterial;
                }).SetEase(Ease.Linear).SetLink(gameObject);
            }
            else
            {
                var initialMaterial = new Material(renderer.materials[materialIndex]);
                var blendMaterial = new Material(renderer.materials[materialIndex]);
                //renderer.material = blendMaterial;

                var materialTween = DOVirtual.Float(0, 1, duration, (value) =>
                {
                    blendMaterial.Lerp(initialMaterial, targetMaterial, value);
                    renderer.materials[materialIndex] = blendMaterial;
                }).SetEase(Ease.Linear).SetLink(gameObject);
            }
        }*/


    }
}

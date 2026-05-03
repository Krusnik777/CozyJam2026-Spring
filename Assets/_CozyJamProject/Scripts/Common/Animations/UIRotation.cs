using UnityEngine;

namespace CozySpringJam
{
    [RequireComponent(typeof(RectTransform))]
    public class UIRotation : MonoBehaviour
    {
        [SerializeField] private float m_rotationSpeed = 15f;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (_rectTransform == null) return;

            _rectTransform.Rotate(0f, 0f, m_rotationSpeed * Time.deltaTime);
        }
    }
}

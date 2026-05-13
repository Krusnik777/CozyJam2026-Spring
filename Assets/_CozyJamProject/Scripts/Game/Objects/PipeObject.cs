using R3;
using UnityEngine;

namespace CozySpringJam.Game.Objects
{
    public class PipeObject : MonoBehaviour
    {
        public enum Mode
        {
            Detect,
            Holder
        }

        [SerializeField] private EnterTrigger m_playerDetector;

        public Mode CurrentMode { get; set; }

        public bool PlayerDetected { get; private set; }
        public bool IsBusy { get; set; }

        private CompositeDisposable _disposables = new();

        public Transform TryGetMovableTransform() => TryDetectMovableObject(Vector3.up, 3f);

        private void Awake()
        {
            m_playerDetector.OnEnter.Subscribe(_ => OnPlayerDetected()).AddTo(_disposables);
            m_playerDetector.OnExit.Subscribe(_ => OnPlayerVanished()).AddTo(_disposables);

            PlayerDetected = false;
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

        private Transform TryDetectMovableObject(Vector3 direction, float distance)
        {
            var origin = transform.position - Vector3.up;
            Ray ray = new Ray(origin, direction.normalized);
            RaycastHit hit;

            int layerMask = 1 << 0;

            bool hitBool = Physics.Raycast(ray, out hit, distance, layerMask);

            Debug.DrawRay(origin, direction.normalized * distance, hitBool ? Color.red : Color.green, 1f);

            if (hitBool && hit.collider != null)
            {
                if (hit.collider.TryGetComponent(out MovableObject movableObject))
                {
                    if (movableObject.IsMoving) return null;

                    return movableObject.transform;
                }
            }

            return null;
        }

        private void OnPlayerVanished()
        {
            PlayerDetected = false;
        }

        private void OnPlayerDetected()
        {
            PlayerDetected = true;
        }
    }
}

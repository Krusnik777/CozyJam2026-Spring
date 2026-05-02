using UnityEngine;

namespace CozySpringJam.Game.Objects
{
    public class PipeObject : MonoBehaviour
    {
        [SerializeField] private MovableObject _movableObject;
        [SerializeField] private PipeObject _secondPipeObject;
        private float _checkInterval = 1f;
        private float _timer;
        private bool _isPressed = false;
        private RaycastHit _hit =  new RaycastHit();

        private void SetMovableObject(MovableObject movableObject)
        {
            _movableObject = movableObject;
        }

        public MovableObject GetMovableObject() => _movableObject;
        
        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer < _checkInterval) return;

            _timer = 0f;
            
            bool hit = ShootRay(Vector3.up, 2f, out _hit);
            
            if (hit && !_isPressed)
            {
                _isPressed = true;
                if (_movableObject != null && _secondPipeObject.GetMovableObject() == null)
                {
                    _secondPipeObject.SetMovableObject(_movableObject);
                    _movableObject.transform.position = new Vector3(_secondPipeObject.transform.position.x, 0, _secondPipeObject.transform.position.z);
                    _movableObject = null;
                    _isPressed = false;
                }
            }
            else if (!hit && _isPressed)
            {
                _isPressed = false;
            }
        }
        
        private bool ShootRay(Vector3 direction, float distance, out RaycastHit hit)
        {
            var origin = transform.position - Vector3.up / 2;
            Ray ray = new Ray(origin, direction.normalized);

            Debug.DrawRay(origin, direction.normalized * distance, Color.red, 1f);

            int layerMask = 1 << 0;

            bool hitBool = Physics.Raycast(ray, out hit, distance, layerMask);

            if (hitBool && hit.collider != null)
            {
                if (hit.collider.TryGetComponent(out MovableObject movableObject))
                {
                    if (_movableObject != movableObject)
                        _movableObject = movableObject;
                    else
                        return false;
                }
            }
            else
            {
                if(_movableObject != null)
                    _movableObject = null;
            }

            return hitBool;
        }
    }
}

using CozySpringJam.Game.Services;
using R3;
using UnityEngine;

namespace CozySpringJam.Game.Objects
{
    public class MechanicalButton : SoundReceiver
    {
        [SerializeField] private LayerMask _mask;
        private float _checkInterval = 0.25f;
        private float _timer;
        private bool _isPressed = false;
        private readonly Subject<bool> _onPress = new();
        public Observable<bool> OnPress => _onPress;
        private SoundService _soundService;

        public override void InitSoundService(SoundService soundService)
        {
            _soundService = soundService;
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer < _checkInterval) return;

            _timer = 0f;
            
            bool hit = ShootRay(Vector3.up, 2f, out _);
            
            if (hit && !_isPressed)
            {
                _isPressed = true;
                _onPress.OnNext(true);
                _soundService.PlayStonePlate();
            }
            else if (!hit && _isPressed)
            {
                _isPressed = false;
                _onPress.OnNext(false);
                _soundService.PlayStonePlate();
            }
        }
        
        private bool ShootRay(Vector3 direction, float distance, out RaycastHit hit)
        {
            var origin = transform.position - Vector3.up / 2;
            Ray ray = new Ray(origin, direction.normalized);

            Debug.DrawRay(origin, direction.normalized * distance, Color.red, 1f);

            int layerMask = 1 << 0;

            return Physics.Raycast(ray, out hit, distance, layerMask);
        }
    }
}

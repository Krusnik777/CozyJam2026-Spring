using CozySpringJam.Game.Services;
using DG.Tweening;
using R3;
using UnityEngine;

namespace CozySpringJam.Game.Objects
{
    public class MechanicalDoor : SoundReceiver
    {
        [SerializeField] private MechanicalButton _mechanicalButton;
        [SerializeField] private Transform _pivot;
        [SerializeField] private float _openDuration;
        [SerializeField] private Vector3 _openRotation;
        
        private Vector3 _baseRotation;
        private SoundService _soundService;
        
        public override void InitSoundService(SoundService soundService)
        {
            _soundService = soundService;
        }

        private void Awake()
        {
            _baseRotation =  _pivot.localEulerAngles;
            _mechanicalButton.OnPress
                .Subscribe(pressed =>
                {
                    if (pressed)
                        Open();
                    else
                        Close();
                })
                .AddTo(this);
        }
        
        private void Open()
        {
            _pivot.DOKill();

            _pivot.DORotate(_openRotation, _openDuration)
                .SetEase(Ease.InOutBack)
                .SetLink(gameObject);

            _soundService.PlayOpenDoor();
        }

        private void Close()
        {
            _pivot.DOKill();

            _pivot.DORotate(_baseRotation, _openDuration)
                .SetEase(Ease.InOutBack)
                .SetLink(gameObject);
            
            _soundService.PlayOpenDoor();
        }
    }
}

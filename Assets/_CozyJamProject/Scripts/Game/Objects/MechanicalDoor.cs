using DG.Tweening;
using R3;
using UnityEngine;

namespace CozySpringJam.Game.Objects
{
    public class MechanicalDoor : MonoBehaviour
    {
        [SerializeField] private MechanicalButton _mechanicalButton;
        [SerializeField] private Transform _pivot;
        [SerializeField] private float _openDuration;
        [SerializeField] private Vector3 _openRotation;
        
        private Vector3 _baseRotation;

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
        }

        private void Close()
        {
            _pivot.DOKill();

            _pivot.DORotate(_baseRotation, _openDuration)
                .SetEase(Ease.InOutBack)
                .SetLink(gameObject);
        }
    }
}

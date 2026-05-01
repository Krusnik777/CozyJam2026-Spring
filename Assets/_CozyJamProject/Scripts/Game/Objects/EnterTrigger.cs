using R3;
using UnityEngine;

namespace CozySpringJam.Game.Objects
{
    public class EnterTrigger : MonoBehaviour
    {
        public Subject<Unit> OnEnter = new();
        public Subject<Unit> OnExit = new();

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out CharacterController player)) return;

            OnEnter.OnNext(Unit.Default);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out CharacterController player)) return;

            OnExit.OnNext(Unit.Default);
        }
    }
}

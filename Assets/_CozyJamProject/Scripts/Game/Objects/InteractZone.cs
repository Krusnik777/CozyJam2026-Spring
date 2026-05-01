using R3;
using UnityEngine;

namespace CozySpringJam
{
    public class InteractZone : MonoBehaviour, IInteractable
    {
        public Subject<Unit> OnEnter = new();
        public void Interact()
        {
            OnEnter.OnNext(Unit.Default);
        }
    }
}

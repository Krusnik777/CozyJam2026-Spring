using R3;
using UnityEngine;

namespace CozySpringJam.Game.Objects
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

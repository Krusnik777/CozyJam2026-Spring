using UnityEngine;

namespace CozySpringJam.Game.Player
{
    public class PlayerAvatarAnimator : MonoBehaviour
    {
        private const string _IsMoving = "IsMoving";
        private const string _OnInteract = "OnInteract";
        private const string _OnSleep = "OnSleep";
        private const float _MovementThreshold = 0.05f;

        [SerializeField] private CharacterController m_characterController;
        [SerializeField] private Animator m_animator;

        public void Interact()
        {
            m_animator.SetTrigger(_OnInteract);
        }

        public void PlayWakeUpAnimation()
        {
            m_animator.SetTrigger(_OnSleep);
        }

        private void Update()
        {
            m_animator.SetBool(_IsMoving, m_characterController.velocity.magnitude >= _MovementThreshold);
        }
    }
}
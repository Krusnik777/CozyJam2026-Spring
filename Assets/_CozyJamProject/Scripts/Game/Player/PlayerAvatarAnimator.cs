using UnityEngine;

namespace CozySpringJam.Game.Player
{
    public class PlayerAvatarAnimator : MonoBehaviour
    {
        private const string _IsMoving = "IsMoving";
        private const float _MovementThreshold = 0.05f;

        [SerializeField] private CharacterController m_characterController;
        [SerializeField] private Animator m_animator;

        private void Update()
        {
            m_animator.SetBool(_IsMoving, m_characterController.velocity.magnitude >= _MovementThreshold);
        }
    }
}
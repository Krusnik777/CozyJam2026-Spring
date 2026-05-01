using CozySpringJam.Utils;
using UnityEngine;

namespace CozySpringJam.Game.Player
{
    public class PlayerAvatarMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController m_characterController;
        [SerializeField] private Transform m_viewTransform;
        [SerializeField] private float m_movementSpeed;
        [SerializeField] private float m_rotationSpeed = 250f;
        [SerializeField] private bool m_doIsometricMovement = true;

        private Vector3 directionControl;
        public Vector3 DirectionControl => directionControl;

        public void SetMoveDirection(Vector3 moveDirection)
        {
            directionControl = moveDirection;
            if (m_doIsometricMovement) directionControl = directionControl.ToIsometric();
            directionControl.Normalize();
        }

        private void Update()
        {
            if (directionControl.magnitude > 0)
            {
                m_characterController.Move(directionControl * m_movementSpeed * Time.deltaTime);
                var targetRotation = Quaternion.LookRotation(directionControl);
                //m_viewTransform.rotation = Quaternion.Slerp(m_viewTransform.rotation, targetRotation, m_rotationSpeed * Time.deltaTime);
                m_viewTransform.rotation = Quaternion.Lerp(m_viewTransform.rotation, targetRotation, m_rotationSpeed * Time.deltaTime);
            }
            else
            {
                m_characterController.Move(Vector3.zero);
            }
        }
    }
}

using CozySpringJam.Game.Services;
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
        [SerializeField] private bool m_inverseInputMovement = true;

        private GameInputService _gameInputService;

        private Vector3 directionControl;

        public void Bind(GameInputService gameInputService) => _gameInputService = gameInputService; 

        public void SetIsometricMovement(bool isIsometric) => m_doIsometricMovement = isIsometric;

        public void Reset() => m_characterController.Move(Vector3.zero);

        public void Teleport(Transform targetPlace)
        {
            m_characterController.Move(Vector3.zero); // just to be safe
            m_characterController.enabled = false;

            transform.position = targetPlace.position;
            m_viewTransform.rotation = targetPlace.rotation;

            m_characterController.enabled = true;
        }

        private void Update()
        {
            GetMoveDirection();

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

        private void GetMoveDirection()
        {
            if (_gameInputService == null) return;

            var moveDirection = _gameInputService.GetMovementInput(m_inverseInputMovement);
            directionControl = moveDirection;
            if (m_doIsometricMovement) directionControl = directionControl.ToIsometric();
            directionControl.Normalize();
        }
    }
}

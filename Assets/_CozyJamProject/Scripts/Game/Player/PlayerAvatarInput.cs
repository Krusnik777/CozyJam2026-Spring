using UnityEngine;

namespace CozySpringJam.Game.Player
{
    public class PlayerAvatarInput : MonoBehaviour
    {
        [SerializeField] private PlayerAvatarMovement m_playerAvatarMovement;
        [SerializeField] private PlayerAvatarInteract m_playerAvatarInteract;
        [SerializeField] private bool m_inverseMovement = true;

        private Vector3 _input;

        /*private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }*/

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                // End for Game ?

                return;
            }

            if(Input.GetButtonDown("Jump") && m_playerAvatarInteract != null)
            {
                m_playerAvatarInteract.CheckEnvironment();

                return;
            }

            _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            if (m_inverseMovement) _input *= -1f;

            m_playerAvatarMovement.SetMoveDirection(_input);
        }
    }
}
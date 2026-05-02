using System;
using R3;
using UnityEngine;

namespace CozySpringJam.Game.Player
{
    public class PlayerAvatarInput : MonoBehaviour, IDisposable
    {
        [SerializeField] private PlayerAvatarMovement m_playerAvatarMovement;
        [SerializeField] private PlayerAvatarInteract m_playerAvatarInteract;
        [SerializeField] private bool m_inverseMovement = true;
        [SerializeField] private ControlsTip m_interactTip;

        public Subject<Unit> OnResetButtonPressed { get; private set;} = new();

        private Vector3 _input;

        private IDisposable _disposable;

        public void Dispose()
        {
            _disposable?.Dispose();
            m_interactTip.Unsubscribe();
        }

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

        private void Awake()
        {
            if (m_playerAvatarInteract != null)
            {
                _disposable?.Dispose(); // just to be safe

                _disposable = m_playerAvatarInteract.DetectedInteractable.Subscribe(detected => m_interactTip.gameObject.SetActive(detected != null));
            }

            m_interactTip.Subscribe();
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                // End for Game ?

                return;
            }

            if (Input.GetButtonDown("Jump") && m_playerAvatarInteract != null)
            {
                m_playerAvatarInteract.CheckEnvironment();

                return;
            }

            if (Input.GetButtonDown("Reset"))
            {
                Debug.Log("TRYING RESET");
                OnResetButtonPressed?.OnNext(Unit.Default);

                return;
            }

            _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            if (m_inverseMovement) _input *= -1f;

            m_playerAvatarMovement.SetMoveDirection(_input);
        }
    }
}
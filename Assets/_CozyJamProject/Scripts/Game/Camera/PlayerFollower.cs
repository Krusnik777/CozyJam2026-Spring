using UnityEngine;

namespace CozySpringJam.Game.Camera
{
    public class PlayerFollower : MonoBehaviour
    {
        [SerializeField] private Transform m_target;
        [SerializeField] private Transform m_followedTarget;

        private void LateUpdate()
        {
            m_target.transform.position = m_followedTarget.transform.position;
        }
    }
}

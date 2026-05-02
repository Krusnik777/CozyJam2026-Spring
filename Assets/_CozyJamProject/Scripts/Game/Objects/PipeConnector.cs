using System.Collections;
using UnityEngine;

namespace CozySpringJam.Game.Objects
{
    public class PipeConnector : MonoBehaviour
    {
        [SerializeField] private float m_checkInterval = 0.25f;
        [SerializeField] private PipeObject m_1stPipe;
        [SerializeField] private PipeObject m_2ndPipe;

        private void Awake()
        {
            StartCoroutine(HandlePipeEndsRoutine());
        }

        private IEnumerator HandlePipeEndsRoutine()
        {
            while(true)
            {
                yield return new WaitForSeconds(m_checkInterval);

                if (m_1stPipe.CurrentMode == PipeObject.Mode.Holder)
                {
                    var checkResult = m_1stPipe.TryGetMovableTransform();

                    if (checkResult == null) m_1stPipe.CurrentMode = PipeObject.Mode.Detect;
                    
                    continue;
                }

                if (m_2ndPipe.CurrentMode == PipeObject.Mode.Holder)
                {
                    var checkResult = m_2ndPipe.TryGetMovableTransform();

                    if (checkResult == null) m_2ndPipe.CurrentMode = PipeObject.Mode.Detect;
                    
                    continue;
                }

                var transformFrom1st = m_1stPipe.TryGetMovableTransform();
                var transformFrom2nd = m_2ndPipe.TryGetMovableTransform();

                if (transformFrom1st != null && transformFrom2nd != null)
                {
                    m_1stPipe.CurrentMode = PipeObject.Mode.Holder;
                    m_2ndPipe.CurrentMode = PipeObject.Mode.Holder;

                    continue;
                }

                if (transformFrom1st != null && !m_2ndPipe.PlayerDetected)
                {
                    //m_1stPipe.CurrentMode = PipeObject.Mode.Holder;
                    m_2ndPipe.CurrentMode = PipeObject.Mode.Holder;

                    transformFrom1st.position = new Vector3(m_2ndPipe.transform.position.x, 0, m_2ndPipe.transform.position.z);
                    
                    continue;
                }

                if (transformFrom2nd != null && !m_1stPipe.PlayerDetected)
                {
                    m_1stPipe.CurrentMode = PipeObject.Mode.Holder;
                    //m_2ndPipe.CurrentMode = PipeObject.Mode.Holder;

                    transformFrom2nd.position = new Vector3(m_1stPipe.transform.position.x, 0, m_1stPipe.transform.position.z);
                    
                    continue;
                }
            }
        }
    }
}

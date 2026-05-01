using CozySpringJam.Game.Services;
using CozySpringJam.Game.SO;
using UnityEngine;

namespace CozySpringJam.Game.Player
{
    public class EventCollector : MonoBehaviour
    {
        public void OnStep()
        {
            SoundService.Instance.PlayFootstep();
            ParticleService.Instance.PlayParticle(ParticleType.StepDust, transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
        }
    }
}

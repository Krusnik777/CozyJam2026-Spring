using CozySpringJam.Game.Services;
using CozySpringJam.Game.SO;
using UnityEngine;

namespace CozySpringJam.Game.Player
{
    public class EventCollector : MonoBehaviour
    {
        private SoundService _soundService;
        private ParticleService _particleService;

        public void Bind(SoundService soundService, ParticleService particleService)
        {
            _soundService = soundService;
            _particleService = particleService;
        }

        public void OnStep()
        {
            _soundService.PlayFootstep();
            _particleService.PlayParticle(ParticleType.StepDust, transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
        }
    }
}

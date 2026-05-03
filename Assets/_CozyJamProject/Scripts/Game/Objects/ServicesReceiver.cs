using CozySpringJam.Game.Services;
using UnityEngine;

namespace CozySpringJam
{
    public abstract class ServicesReceiver : MonoBehaviour
    {
        protected SoundService _soundService;
        protected ParticleService _particleService;

        public virtual void Bind(SoundService soundService, ParticleService particleService)
        {
            _soundService = soundService;
            _particleService = particleService;
        }
    }
}

using CozySpringJam.Game.Services;
using UnityEngine;

namespace CozySpringJam.Game.Objects
{
    public abstract class ParticleReceiver : MonoBehaviour
    {
        public abstract void InitParticleService(ParticleService particleService);
    }
}

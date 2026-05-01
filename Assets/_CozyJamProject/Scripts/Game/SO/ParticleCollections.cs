using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CozySpringJam.Game.SO
{
    [CreateAssetMenu(fileName = "ParticleCollection", menuName = "Scriptable Objects/ParticleCollection")]
    public class ParticleCollections : ScriptableObject
    {
        public Particle[] particles;

        [System.Serializable]
        public class Particle
        {
            public ParticleType type;
            public Transform prefab;
        }

        public Transform GetParticle(ParticleType type)
        {
            foreach (var particle in particles)
            {
                if (particle.type == type)
                {
                    return particle.prefab;
                }
            }

            return null;
        }
        
        public List<ParticleCollections.Particle> GetParticleList()
        {
            return particles.ToList();
        }
    }
    
    public enum ParticleType
    {
        Dust = 0,
    }
}

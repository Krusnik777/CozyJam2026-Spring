using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CozySpringJam.Game.SO;
using Lean.Pool;
using UnityEngine;

namespace CozySpringJam.Game.Services
{
    public class ParticleService
    {
        private Dictionary<ParticleType, Transform> _particlePrefabs = new Dictionary<ParticleType, Transform>();
        private readonly HashSet<Transform> _activeParticles = new(1000);
        private ParticleCollections _particleCollections;
        public static ParticleService Instance { get; private set; }

        public ParticleService(ParticleCollections particleCollections)
        {
            Instance = this;
            
            _particleCollections = particleCollections;
            
            _particlePrefabs.Clear();
            foreach (var particle in _particleCollections.GetParticleList())
            {
                if(particle != null)
                    if (particle.prefab != null)
                        _particlePrefabs[particle.type] = particle.prefab;
            }
        }
        
        public Transform PlayParticle(
            ParticleType type,
            Vector3 position,
            Quaternion rotation,
            float? customDuration = null,
            Transform parent = null)
        {
            if (!_particlePrefabs.TryGetValue(type, out var prefab) || prefab == null)
            {
                Debug.LogWarning($"No particle prefab found for type: {type}");
                return null;
            }

            if (parent != null && parent.Equals(null))
                parent = null;

            var effect = LeanPool.Spawn(prefab, position, rotation, parent);
            if (effect == null)
                return null;

            _activeParticles.Add(effect);

            float duration = customDuration ?? 0.5f;
            var ps = effect.GetComponent<ParticleSystem>();
            if (ps != null && customDuration == null)
            {
                duration = ps.main.duration + ps.main.startLifetime.constantMax;
            }

            LeanPool.Despawn(effect, duration);
            _ = RemoveAfterDelay(effect, duration);

            return effect;
        }
        
        public RectTransform PlayParticleUI(
            ParticleType type,
            Vector2 position,
            Quaternion rotation,
            float? customDuration = null,
            RectTransform parent = null)
        {
            if (!_particlePrefabs.TryGetValue(type, out var prefab) || prefab == null)
            {
                Debug.LogWarning($"No particle prefab found for type: {type}");
                return null;
            }
            
            if (parent != null && parent.Equals(null))
                parent = null;
            
            var effect = LeanPool.Spawn(prefab, Vector3.zero, rotation, parent) as RectTransform;
            if (effect == null)
                return null;

            _activeParticles.Add(effect);
            
            effect.position = position;
            effect.localRotation = rotation;

            float duration = customDuration ?? 0.5f;
            var ps = effect.GetComponent<ParticleSystem>();
            if (ps != null && customDuration == null)
            {
                duration = ps.main.duration + ps.main.startLifetime.constantMax;
            }

            LeanPool.Despawn(effect, duration);
            _ = RemoveAfterDelay(effect, duration);

            return effect;
        }
        
        public async Task PlayParticleSequence(
            List<(ParticleType type, Vector3 pos, Quaternion rot, Transform parent, float? customDuration)> sequence,
            float delayBetween = 0f,
            Action onComplete = null)
        {
            foreach (var entry in sequence)
            {
                PlayParticle(entry.type, entry.pos, entry.rot, entry.customDuration, entry.parent);
                if (delayBetween > 0)
                    await Task.Delay(TimeSpan.FromSeconds(delayBetween));
            }

            onComplete?.Invoke();
        }

        private async Task RemoveAfterDelay(Transform effect, float delay)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay + 0.1f));
            if (effect != null)
                _activeParticles.Remove(effect);
        }

        public void Clear()
        {
            foreach (var effect in _activeParticles)
            {
                if (effect != null)
                    LeanPool.Despawn(effect);
            }

            _activeParticles.Clear();
        }
    }
}

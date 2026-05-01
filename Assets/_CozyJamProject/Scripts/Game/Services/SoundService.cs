using UnityEngine;

namespace CozySpringJam.Game.Services
{
    public class SoundService
    {
        private readonly AudioSource _audioSource;
        private readonly AudioSource _backgroundSource;
        public static SoundService Instance { get; private set; }

        public SoundService(AudioSource audioSource, AudioSource backgroundSource)
        {
            Instance = this;
            _audioSource = audioSource;
            _backgroundSource = backgroundSource;
        }

        private void Play(string soundName, float volume = 1)
        {
            var clip = Resources.Load<AudioClip>($"Sounds/{soundName}");

            if (clip == null)
            {
                Debug.LogWarning($"Sound not found: {soundName}");
                return;
            }

            _audioSource.PlayOneShot(clip, volume);
        }

        public void PlayFootstep()
        {
            int step = Random.Range(1, 5);
            Play($"Footstep_{step}");
        }
        public void PlaySadMeow()
        {
            Play($"SadMeow");
        }
        
        public void PlayMoveObject()
        {
            Play($"MoveObject");
        }

        public void PlayBackgroundMusic()
        {
            var clip = Resources.Load<AudioClip>($"Sounds/BackgroundMusic");

            if (clip == null)
            {
                Debug.LogWarning($"Sound not found: BackgroundMusic");
                return;
            }
            _backgroundSource.loop = true;
            _backgroundSource.clip = clip;
            _backgroundSource.volume = 0.1f;
            _backgroundSource.Play();
        }
        
    }
}

using System;
using System.Collections;
using CozySpringJam.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CozySpringJam.Game.Services
{
    public class SoundService
    {
        private readonly AudioSource _audioSource;
        private readonly AudioSource _backgroundSource;
        private Coroutines _coroutines;
        private Coroutine _fadeCoroutine;
        private float _savedBackgroundMusicVolume;
        private string[] _playlist;
        private int _currentTrackIndex;
        private Coroutine _playlistCoroutine;

        public SoundService(AudioSource audioSource, AudioSource backgroundSource, Coroutines coroutines)
        {
            _audioSource = audioSource;
            _backgroundSource = backgroundSource;
            _coroutines = coroutines;
        }

        public void PlayFootstep()
        {
            int step = Random.Range(1, 5);
            Play($"Footstep_{step}");
        }
        public void PlaySadMeow()
        {
            Play("SadMeow", 0.5f);
        }
        
        public void PlayMoveObject()
        {
            Play("MoveObject");
        }
        
        public void PlayStonePlate()
        {
            Play("StonePlate");
        }

        public void PlayOpenDoor()
        {
            Play("OpenDoor");
        }
        
        public void PlayPuff()
        {
            Play("Puff");
        }

        public void PlayWoop()
        {
            Play("Woop");
        }
        
        public void PlaySuccessful()
        {
            PauseBackgroundMusic();
            Play($"Successful", 0.35f);
            InvokeDelayed(() => ContinueBackgroundMusic(),7f);
        }

        public void PlayBackgroundMusic(bool withFade = true)
        {
            var clip = Resources.Load<AudioClip>($"Sounds/BackgroundMusic");

            if (clip == null)
            {
                Debug.LogWarning($"Sound not found: BackgroundMusic");
                return;
            }
            
            _backgroundSource.loop = true;
            _backgroundSource.clip = clip;
            _backgroundSource.volume = 0.05f;
            _savedBackgroundMusicVolume = _backgroundSource.volume;

            if (withFade)
            {
                _backgroundSource.volume = 0f;

                StartFade(_savedBackgroundMusicVolume, 1f, stopAfterFade: false);
            }

            _backgroundSource.Play();
        }

        public void StopBackgroundMusic()
        {
            _backgroundSource.Stop();
        }

        public void PauseBackgroundMusic(float duration = 1f)
        {
            StartFade(0f, duration, stopAfterFade: true);
        }

        public void ContinueBackgroundMusic(float duration = 1f)
        {
            if (!_backgroundSource.isPlaying)
                _backgroundSource.Play();

            StartFade(_savedBackgroundMusicVolume, duration, stopAfterFade: false);
        }
        
        public void Play(string soundName, float volume = 1)
        {
            var clip = Resources.Load<AudioClip>($"Sounds/{soundName}");

            if (clip == null)
            {
                Debug.LogWarning($"Sound not found: {soundName}");
                return;
            }

            _audioSource.PlayOneShot(clip, volume);
        }

        private void InvokeDelayed(Action action, float delay)
        {
            _coroutines.StartCoroutine(InvokeDelayedRoutine(action, delay));
        }

        private IEnumerator InvokeDelayedRoutine(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        private void StartFade(float targetVolume, float duration, bool stopAfterFade)
        {
            if (_fadeCoroutine != null)
                _coroutines.StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = _coroutines.StartCoroutine(
                FadeRoutine(targetVolume, duration, stopAfterFade));
        }

        private IEnumerator FadeRoutine(float targetVolume, float duration, bool stopAfterFade)
        {
            float startVolume = _backgroundSource.volume;
            float time = 0f;
            
            if (targetVolume == 0f)
                _savedBackgroundMusicVolume = startVolume;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;

                _backgroundSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
                yield return null;
            }

            _backgroundSource.volume = targetVolume;

            if (stopAfterFade && Mathf.Approximately(targetVolume, 0f))
            {
                _backgroundSource.Stop();
            }
        }

        #region PlayList

        public void PlayBackgroundPlaylist(string[] trackNames, bool withFade = false)
        {
            if (trackNames == null || trackNames.Length == 0)
            {
                Debug.LogWarning("Playlist is empty");
                return;
            }

            _playlist = trackNames;
            _currentTrackIndex = 0;
            _backgroundSource.volume = 0.05f;
            _savedBackgroundMusicVolume = _backgroundSource.volume;
            if (_playlistCoroutine != null)
                _coroutines.StopCoroutine(_playlistCoroutine);

            _playlistCoroutine = _coroutines.StartCoroutine(PlaylistRoutine(withFade));
        }
        
        private IEnumerator PlaylistRoutine(bool withFade)
        {
            while (true)
            {
                yield return PlayTrack(_playlist[_currentTrackIndex], withFade);

                _currentTrackIndex = (_currentTrackIndex + 1) % _playlist.Length;
            }
        }
        
        private IEnumerator PlayTrack(string trackName, bool withFade)
        {
            var clip = Resources.Load<AudioClip>($"Sounds/{trackName}");

            if (clip == null)
            {
                Debug.LogWarning($"Track not found: {trackName}");
                yield break;
            }

            _backgroundSource.clip = clip;
            _backgroundSource.loop = false;
            _backgroundSource.volume = _savedBackgroundMusicVolume;

            if (withFade)
            {
                _backgroundSource.volume = 0f;
                _backgroundSource.Play();
                StartFade(_savedBackgroundMusicVolume, 1f, false);
            }
            else
            {
                _backgroundSource.Play();
            }

            yield return new WaitForSeconds(clip.length);
        }
        
        public void StopPlaylist()
        {
            if (_playlistCoroutine != null)
            {
                _coroutines.StopCoroutine(_playlistCoroutine);
                _playlistCoroutine = null;
            }

            StopBackgroundMusic();
        }
        

        #endregion
    }
}

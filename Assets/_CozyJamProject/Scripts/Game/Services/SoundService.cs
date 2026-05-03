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
        private readonly AudioSource _loopedAudioSource;
        private readonly AudioSource _backgroundSource;

        private Coroutines _coroutines;
        private Coroutine _fadeBGMCoroutine;
        private Coroutine _fadeSoundCoroutine;

        private float _savedBackgroundMusicVolume;
        private float _savedLoopSoundVolume;

        private string[] _playlist;
        private int _currentTrackIndex;
        private Coroutine _playlistCoroutine;

        public SoundService(AudioSource audioSource, AudioSource loopedAudioSource, AudioSource backgroundSource, Coroutines coroutines)
        {
            _audioSource = audioSource;
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;

            _loopedAudioSource = loopedAudioSource;
            _loopedAudioSource.playOnAwake = false;
            _loopedAudioSource.loop = true;

            _backgroundSource = backgroundSource;
            _backgroundSource.playOnAwake = false;

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

        public void PlayCatSleep()
        {
            //Play("CatSleep");
            PlayLoop("CatSleep", 0.5f);
        }

        public void PlayWakeUpMew(float delay = 6.5f)
        {
            if (delay <= 0)
            {
                Play("WakeUpMew", 0.35f);

                return;
            }

            InvokeDelayed(delay, () => Play("WakeUpMew", 0.35f));
        }

        public void PlaySuccessful()
        {
            PauseBackgroundMusic();
            Play($"Successful", 0.35f);
            InvokeDelayed(7f, () => ContinueBackgroundMusic());
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

                StartFadeBGM(_savedBackgroundMusicVolume, 1f, stopAfterFade: false);
            }

            _backgroundSource.Play();
        }

        public void StopBackgroundMusic()
        {
            _backgroundSource.Stop();
        }

        public void PauseBackgroundMusic(float duration = 1f)
        {
            StartFadeBGM(0f, duration, stopAfterFade: true);
        }

        public void ContinueBackgroundMusic(float duration = 1f)
        {
            if (!_backgroundSource.isPlaying)
                _backgroundSource.Play();

            StartFadeBGM(_savedBackgroundMusicVolume, duration, stopAfterFade: false);
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

        public void PlayLoop(string soundName, float volume = 1f, bool withFade = true)
        {
            var clip = Resources.Load<AudioClip>($"Sounds/{soundName}");

            if (clip == null)
            {
                Debug.LogWarning($"Sound not found: {soundName}");
                return;
            }

            _loopedAudioSource.clip = clip;
            _loopedAudioSource.volume = volume;
            _savedLoopSoundVolume = volume;

            if (withFade)
            {
                _loopedAudioSource.volume = 0f;

                StartFadeLoopSound(_savedLoopSoundVolume, 1f, stopAfterFade: false);
            }

            _loopedAudioSource.Play();
        }

        public void StopLoopedSound()
        {
            StartFadeLoopSound(0, 1f, true);
        }

        private void InvokeDelayed(float delay, Action action)
        {
            _coroutines.StartCoroutine(InvokeDelayedRoutine(delay, action));
        }

        private IEnumerator InvokeDelayedRoutine(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        private void StartFadeBGM(float targetVolume, float duration, bool stopAfterFade)
        {
            if (_fadeBGMCoroutine != null)
                _coroutines.StopCoroutine(_fadeBGMCoroutine);

            _fadeBGMCoroutine = _coroutines.StartCoroutine(
                FadeBGMRoutine(targetVolume, duration, stopAfterFade));
        }

        private IEnumerator FadeBGMRoutine(float targetVolume, float duration, bool stopAfterFade)
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

        private void StartFadeLoopSound(float targetVolume, float duration, bool stopAfterFade)
        {
            if (_fadeSoundCoroutine != null)
                _coroutines.StopCoroutine(_fadeSoundCoroutine);

            _fadeSoundCoroutine = _coroutines.StartCoroutine(
                FadeSoundRoutine(targetVolume, duration, stopAfterFade));
        }

        private IEnumerator FadeSoundRoutine(float targetVolume, float duration, bool stopAfterFade)
        {
            float startVolume = _loopedAudioSource.volume;
            float time = 0f;

            if (targetVolume == 0f)
                _savedLoopSoundVolume = startVolume;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;

                _loopedAudioSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
                yield return null;
            }

            _loopedAudioSource.volume = targetVolume;

            if (stopAfterFade && Mathf.Approximately(targetVolume, 0f))
            {
                _loopedAudioSource.Stop();
                _loopedAudioSource.clip = null;
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
                StartFadeBGM(_savedBackgroundMusicVolume, 1f, false);
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

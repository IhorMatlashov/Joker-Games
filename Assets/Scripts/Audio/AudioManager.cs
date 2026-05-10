using Betting;
using Core;
using Data;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioLibrarySo library;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource loopSource;
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioMixerGroup musicGroup;
        [SerializeField] private AudioMixerGroup sfxGroup;

        private void Reset() => sfxSource = GetComponent<AudioSource>();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (sfxSource != null && sfxGroup != null) sfxSource.outputAudioMixerGroup = sfxGroup;
            if (loopSource != null && musicGroup != null) loopSource.outputAudioMixerGroup = musicGroup;
            ApplySavedVolumes();
        }

        public void ApplySavedVolumes()
        {
            if (mixer == null) return;
            mixer.SetFloat("MusicVolume", LinearToDb(SaveSystem.MusicVolume));
            mixer.SetFloat("SfxVolume", LinearToDb(SaveSystem.SfxVolume));
        }

        private static float LinearToDb(float linear) =>
            linear <= 0.0001f ? -80f : Mathf.Log10(Mathf.Clamp01(linear)) * 20f;

        private void OnEnable()
        {
            EventBus.OnSpinStarted   += OnSpinStarted;
            EventBus.OnSpinEnded     += OnSpinEnded;
            EventBus.OnRoundResolved += OnResolved;
            EventBus.OnBetPlaced     += OnBetPlaced;
            EventBus.OnBetsCleared   += OnBetsCleared;
        }

        private void OnDisable()
        {
            EventBus.OnSpinStarted   -= OnSpinStarted;
            EventBus.OnSpinEnded     -= OnSpinEnded;
            EventBus.OnRoundResolved -= OnResolved;
            EventBus.OnBetPlaced     -= OnBetPlaced;
            EventBus.OnBetsCleared   -= OnBetsCleared;
        }

        public void PlayFretClack()
        {
            if (library == null || sfxSource == null) return;
            var clips = library.fretClacks;
            if (clips == null || clips.Length == 0) return;

            var clip = clips[Random.Range(0, clips.Length)];
            if (clip == null) return;

            var jitter = library.fretClackPitchJitter;
            sfxSource.pitch = 1f + Random.Range(-jitter, jitter);
            sfxSource.PlayOneShot(clip);
            sfxSource.pitch = 1f;
        }

        private void OnSpinStarted()
        {
            if (library == null) return;
            float jitter = library.spinPitchJitter;
            float spinPitch = 1f + Random.Range(-jitter, jitter);

            if (loopSource != null && library.wheelSpinLoop != null)
            {
                loopSource.clip = library.wheelSpinLoop;
                loopSource.loop = true;
                loopSource.pitch = spinPitch;
                loopSource.Play();
            }

            float ballPitch = 1f + Random.Range(-jitter, jitter);
            if (sfxSource != null && library.ballRoll != null)
            {
                sfxSource.pitch = ballPitch;
                sfxSource.PlayOneShot(library.ballRoll);
                sfxSource.pitch = 1f;
            }
        }

        private void OnSpinEnded(int _, PocketColor __)
        {
            if (loopSource != null && loopSource.isPlaying) loopSource.Stop();
            if (sfxSource != null) sfxSource.Stop();
            if (library != null) PlayOneShotFlat(library.ballLand);
        }

        private void OnResolved(SpinResult r)
        {
            if (library == null) return;
            PlayOneShotFlat(r.PlayerWon ? library.win : library.lose);
        }

        private void OnBetPlaced(Bet _)
        {
            if (library == null || sfxSource == null) return;

            var clips = library.chipPlaceSounds;
            AudioClip clip = clips != null && clips.Length > 0
                ? clips[Random.Range(0, clips.Length)]
                : library.chipPlace;
            if (clip == null) return;

            float jitter = library.chipPitchJitter;
            sfxSource.pitch = 1f + Random.Range(-jitter, jitter);
            sfxSource.PlayOneShot(clip);
            sfxSource.pitch = 1f;
        }

        private void OnBetsCleared() { if (library != null) PlayOneShotFlat(library.chipClear); }

        private void PlayOneShotFlat(AudioClip clip)
        {
            if (clip == null || sfxSource == null) return;
            sfxSource.pitch = 1f;
            sfxSource.PlayOneShot(clip);
        }
    }
}

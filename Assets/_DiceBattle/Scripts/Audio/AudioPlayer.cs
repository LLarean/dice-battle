using DiceBattle.Data;
using DiceBattle.Events;
using DiceBattle.Global;
using GameSignals;
using UnityEngine;

namespace DiceBattle.Audio
{
    public class AudioPlayer : MonoBehaviour, ISoundHandler
    {
        [SerializeField] private SoundConfig _soundConfig;
        [Space]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;

        public void PlayMusic(SoundType soundType)
        {
            if (_soundConfig.TryGetAudioClip(soundType, out AudioClip audioClip) == false)
            {
                return;
            }

            if (_musicSource.isPlaying)
            {
                LeanTween.value(gameObject,
                        _musicSource.volume,
                        0f,
                        0.5f)
                    .setOnUpdate(v => _musicSource.volume = v)
                    .setOnComplete(() => SwapTrack(audioClip));
            }
            else
            {
                SwapTrack(audioClip);
            }
        }

        public void PlaySound(SoundType soundType)
        {
            if (_soundConfig.TryGetAudioClip(soundType, out AudioClip audioClip) == false)
            {
                return;
            }

            _sfxSource.pitch = Random.Range(0.9f, 1.1f);
            _sfxSource.PlayOneShot(audioClip);
        }

        public void SetMusicVolume(float value)
        {
            _musicSource.volume = value;
            GameSettings.SetMusicVolume(value);
        }

        public void SetSoundVolume(float value)
        {
            _sfxSource.volume = value;
            GameSettings.SetSoundVolume(value);
        }

        private void Awake() => SignalSystem.Subscribe(this);

        private void Start()
        {
            _musicSource.loop = true;
            _sfxSource.loop = false;

            _musicSource.volume = GameSettings.MusicVolume;
            _sfxSource.volume = GameSettings.SoundVolume;
        }

        private void OnDestroy() => SignalSystem.Unsubscribe(this);

        private void SwapTrack(AudioClip audioClip)
        {
            _musicSource.clip = audioClip;
            _musicSource.volume = 1f;
            _musicSource.Play();
        }
    }
}

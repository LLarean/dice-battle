using System.Collections.Generic;
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
        [SerializeField] private AudioSource _fadingMusicSource;
        [SerializeField] private AudioSource _sfxSource;

        private const float _musicFadeDuration = 1.5f;
        private const float _duckFadeDuration = 0.25f;
        private const float _duckedMusicVolume = 0.2f;

        private readonly Dictionary<AudioClip, float> _musicPositions = new();

        private float _musicVolume;
        private float _duckedUntil;

        public void PlayMusic(SoundType soundType)
        {
            if (_soundConfig.TryGetAudioClip(soundType, out AudioClip audioClip) == false || _musicSource.clip == audioClip)
            {
                return;
            }

            (_musicSource, _fadingMusicSource) = (_fadingMusicSource, _musicSource);

            // The requested track may still be fading out on this source; then it just fades back in.
            if (_musicSource.clip == audioClip && _musicSource.isPlaying)
            {
                return;
            }

            StopMusic(_musicSource);

            _musicSource.clip = audioClip;
            _musicSource.volume = 0f;
            _musicSource.Play();
            _musicSource.time = _musicPositions.GetValueOrDefault(audioClip);
        }

        public void PlaySound(SoundType soundType)
        {
            if (_soundConfig.TryGetAudioClip(soundType, out AudioClip audioClip) == false)
            {
                return;
            }

            _sfxSource.pitch = Random.Range(0.9f, 1.1f);
            _sfxSource.PlayOneShot(audioClip);

            if (soundType is SoundType.Victory or SoundType.Defeat or SoundType.Reward)
            {
                _duckedUntil = Time.unscaledTime + audioClip.length;
            }
        }

        public void SetMusicVolume(float value)
        {
            _musicVolume = value;
            _musicSource.volume = value;
            _fadingMusicSource.volume = Mathf.Min(_fadingMusicSource.volume, value);
            GameSettings.SetMusicVolume(value);
        }

        public void SetSoundVolume(float value)
        {
            _sfxSource.volume = value;
            GameSettings.SetSoundVolume(value);
        }

        private void Awake()
        {
            _musicVolume = GameSettings.MusicVolume;
            SignalSystem.Subscribe(this);
        }

        private void Start()
        {
            _musicSource.loop = true;
            _fadingMusicSource.loop = true;
            _sfxSource.loop = false;

            _sfxSource.volume = GameSettings.SoundVolume;
        }

        private void Update()
        {
            float step = _musicVolume * Time.unscaledDeltaTime / _musicFadeDuration;

            // Ducks quickly under a jingle and comes back at the usual fade speed.
            bool isDucked = Time.unscaledTime < _duckedUntil;
            float targetVolume = isDucked ? _musicVolume * _duckedMusicVolume : _musicVolume;
            bool isDucking = isDucked && _musicSource.volume > targetVolume;
            float musicStep = isDucking ? _musicVolume * Time.unscaledDeltaTime / _duckFadeDuration : step;

            _musicSource.volume = Mathf.MoveTowards(_musicSource.volume, targetVolume, musicStep);
            _fadingMusicSource.volume = Mathf.MoveTowards(_fadingMusicSource.volume, 0f, step);

            if (_fadingMusicSource.volume <= 0f)
            {
                StopMusic(_fadingMusicSource);
            }
        }

        private void OnDestroy() => SignalSystem.Unsubscribe(this);

        private void StopMusic(AudioSource source)
        {
            if (source.isPlaying == false)
            {
                return;
            }

            _musicPositions[source.clip] = source.time;
            source.Stop();
        }
    }
}

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

        private readonly Dictionary<AudioClip, float> _musicPositions = new();

        private float _musicVolume;

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

            _musicSource.volume = Mathf.MoveTowards(_musicSource.volume, _musicVolume, step);
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

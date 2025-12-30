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
        [Header("---AUDIO SOURCES---")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;
        [Space]
        [SerializeField] private SoundConfig _soundConfig;

        [Header("---MUSIC CLIPS---")]
        [SerializeField] private AudioClip _backgroundMusic;

        [Header("---OPTIONS---")]
        [SerializeField] private bool _playMusicOnStart = true;

        public void PlaySound(SoundType soundType)
        {
            if (_soundConfig.TryGetAudioClip(soundType, out AudioClip audioClip))
            {
                _sfxSource.PlayOneShot(audioClip);
            }
        }

        public void SetMusicValue(float value)
        {
            _musicSource.volume = value;
            GameSettings.SetMusicVolume(value);
        }

        public void SetSoundValue(float value)
        {
            _sfxSource.volume = value;
            GameSettings.SetSoundVolume(value);
        }

        private void Awake() => SignalSystem.Subscribe(this);

        private void Start()
        {
            _musicSource.loop = true;

            if (_playMusicOnStart && _backgroundMusic != null)
            {
                _musicSource.clip = _backgroundMusic;
                _musicSource.Play();
            }

            _sfxSource.loop = false;

            _musicSource.volume = GameSettings.MusicVolume;
            _sfxSource.volume = GameSettings.SoundVolume;
        }

        private void OnDestroy() => SignalSystem.Unsubscribe(this);
    }
}

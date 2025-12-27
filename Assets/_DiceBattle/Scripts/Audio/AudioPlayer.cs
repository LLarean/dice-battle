using System.Collections.Generic;
using DiceBattle.Data;
using GameSignals;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DiceBattle.Audio
{
    public class AudioPlayer : MonoBehaviour, ISoundHandler
    {
        [Header("---AUDIO SOURCES---")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;
        [Space]
        [SerializeField] private SoundConfig _soundConfig;

        [Header("---AUDIO CLIPS---")]
        [Header("Actions with UI")]
        [SerializeField] private AudioClip _click;

        [Header("Actions with dice")]
        [SerializeField] private List<AudioClip> _diceGrab;
        [SerializeField] private List<AudioClip> _diceShake;
        [SerializeField] private List<AudioClip> _diceThrow;
        [SerializeField] private List<AudioClip> _dieThrow;
        [SerializeField] private List<AudioClip> _playerAttack;
        [SerializeField] private List<AudioClip> _playerHeal;

        [Header("Actions of enemies")]
        [SerializeField] private List<AudioClip> _slimeAttack;
        [SerializeField] private AudioClip _enemyHitClip;
        [SerializeField] private AudioClip _enemyDefeatedClip;
        [SerializeField] private AudioClip _enemySpawnClip;
        [SerializeField] private AudioClip _gameOverClip;

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

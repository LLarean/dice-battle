using System.Collections.Generic;
using GameSignals;
using UnityEngine;

namespace DiceBattle.Audio
{
    public class AudioPlayer : MonoBehaviour, ISoundHandler
    {
        [Header("Audio Sources")] [SerializeField]
        private AudioSource _musicSource;

        [SerializeField] private AudioSource _sfxSource;

        [Header("Audio Clips")] [SerializeField]
        private AudioClip _diceRollClip;

        [SerializeField] private AudioClip _diceRerollClip;
        [SerializeField] private AudioClip _diceLockClip;
        [SerializeField] private AudioClip _playerAttackClip;
        [SerializeField] private AudioClip _playerHealClip;
        [SerializeField] private AudioClip _playerHitClip;
        [SerializeField] private AudioClip _enemyHitClip;
        [SerializeField] private AudioClip _enemyDefeatedClip;
        [SerializeField] private AudioClip _enemySpawnClip;
        [SerializeField] private AudioClip _gameOverClip;
        [Space]
        [SerializeField] private List<AudioClip> _diceGrab;
        [SerializeField] private List<AudioClip> _diceShake;
        [SerializeField] private List<AudioClip> _diceThrow;
        [SerializeField] private List<AudioClip> _dieThrow;
        [Space]
        [Header("Music")] [SerializeField]
        private AudioClip _backgroundMusic;

        [SerializeField] private bool _playMusicOnStart = true;

        public void PlaySound(SoundType soundType)
        {
            AudioClip clip = GetClipByType(soundType);

            if (clip != null && _sfxSource != null)
            {
                _sfxSource.PlayOneShot(clip);
            }
        }

        public void SetMusicEnabled(bool enabled)
        {
            if (_musicSource == null)
                return;

            if (enabled && !_musicSource.isPlaying)
            {
                _musicSource.Play();
            }
            else if (!enabled && _musicSource.isPlaying)
            {
                _musicSource.Pause();
            }
        }

        public void SetMusicVolume(float volume)
        {
            if (_musicSource != null)
            {
                _musicSource.volume = Mathf.Clamp01(volume);
            }
        }

        public void SetSFXVolume(float volume)
        {
            if (_sfxSource != null)
            {
                _sfxSource.volume = Mathf.Clamp01(volume);
            }
        }

        private AudioClip GetClipByType(SoundType soundType)
        {
            switch (soundType)
            {
                case SoundType.DiceGrab:
                    return _diceGrab[Random.Range(0, _diceGrab.Count)];
                case SoundType.DiceShake:
                    return _diceShake[Random.Range(0, _diceShake.Count)];
                case SoundType.DiceThrow:
                    return _diceThrow[Random.Range(0, _diceThrow.Count)];
                case SoundType.DieThrow:
                    return _dieThrow[Random.Range(0, _dieThrow.Count)];

                case SoundType.DiceRoll:
                    return _diceRollClip;
                case SoundType.DiceReroll:
                    return _diceRerollClip;
                case SoundType.DiceLock:
                    return _diceLockClip;
                case SoundType.PlayerAttack:
                    return _playerAttackClip;
                case SoundType.PlayerHeal:
                    return _playerHealClip;
                case SoundType.PlayerHit:
                    return _playerHitClip;
                case SoundType.EnemyHit:
                    return _enemyHitClip;
                case SoundType.EnemyDefeated:
                    return _enemyDefeatedClip;
                case SoundType.EnemySpawn:
                    return _enemySpawnClip;
                case SoundType.GameOver:
                    return _gameOverClip;
                default:
                    return null;
            }
        }

        #region Unity lifecycle

        private void Start()
        {
            _musicSource.loop = true;

            if (_playMusicOnStart && _backgroundMusic != null)
            {
                _musicSource.clip = _backgroundMusic;
                _musicSource.Play();
            }

            _sfxSource.loop = false;
        }

        private void OnEnable()
        {
            SignalSystem.Subscribe(this);
        }

        private void OnDisable()
        {
            SignalSystem.Unsubscribe(this);
        }

        #endregion
    }
}

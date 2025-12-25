using System.Collections.Generic;
using DiceBattle.Global;
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
            AudioClip clip = GetClipByType(soundType);
            _sfxSource.PlayOneShot(clip);
        }

        public void ChangeMusicValue(float value)
        {
            _musicSource.volume = value;
            GameSettings.SetMusicVolume(value);
        }

        public void ChangeSoundValue(float value)
        {
            _sfxSource.volume = value;
            GameSettings.SetSoundVolume(value);
        }

        private AudioClip GetClipByType(SoundType soundType)
        {
            switch (soundType)
            {
                case SoundType.Click:
                    return _click;

                case SoundType.DiceGrab:
                    return _diceGrab[Random.Range(0, _diceGrab.Count)];
                case SoundType.DiceShake:
                    return _diceShake[Random.Range(0, _diceShake.Count)];
                case SoundType.DiceThrow:
                    return _diceThrow[Random.Range(0, _diceThrow.Count)];
                case SoundType.DieThrow:
                    return _dieThrow[Random.Range(0, _dieThrow.Count)];

                case SoundType.PlayerAttack:
                    return _playerAttack[Random.Range(0, _playerAttack.Count)];
                case SoundType.PlayerHeal:
                    return _playerHeal[Random.Range(0, _playerHeal.Count)];

                case SoundType.SlimeAttack:
                    return _slimeAttack[Random.Range(0, _slimeAttack.Count)];
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

        #endregion
    }
}

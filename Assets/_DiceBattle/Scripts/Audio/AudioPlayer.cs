using UnityEngine;

namespace DiceBattle.Audio
{
    /// <summary>
    /// Simple audio controller for managing music and sound effects
    /// Uses two AudioSource components: one for music, another for sound effects
    /// </summary>
    public class AudioPlayer : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip _diceRollClip;
        [SerializeField] private AudioClip _diceRerollClip;
        [SerializeField] private AudioClip _diceLockClip;
        [SerializeField] private AudioClip _playerAttackClip;
        [SerializeField] private AudioClip _playerHealClip;
        [SerializeField] private AudioClip _playerHitClip;
        [SerializeField] private AudioClip _enemyHitClip;
        [SerializeField] private AudioClip _enemyDefeatedClip;
        [SerializeField] private AudioClip _enemySpawnClip;
        [SerializeField] private AudioClip _gameOverClip;

        [Header("Music")]
        [SerializeField] private AudioClip _backgroundMusic;
        [SerializeField] private bool _playMusicOnStart = true;

        private void Start()
        {
            // Configure audio sources
            if (_musicSource != null)
            {
                _musicSource.loop = true;
                
                if (_playMusicOnStart && _backgroundMusic != null)
                {
                    _musicSource.clip = _backgroundMusic;
                    _musicSource.Play();
                }
            }

            if (_sfxSource != null)
            {
                _sfxSource.loop = false;
            }
        }

        /// <summary>
        /// Play sound by type (called from SignalSystem)
        /// </summary>
        public void PlaySound(SoundType soundType)
        {
            AudioClip clip = GetClipByType(soundType);
            
            if (clip != null && _sfxSource != null)
            {
                _sfxSource.PlayOneShot(clip);
            }
        }

        /// <summary>
        /// Enable/disable music
        /// </summary>
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

        /// <summary>
        /// Set music volume (0-1)
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            if (_musicSource != null)
            {
                _musicSource.volume = Mathf.Clamp01(volume);
            }
        }

        /// <summary>
        /// Set sound effects volume (0-1)
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            if (_sfxSource != null)
            {
                _sfxSource.volume = Mathf.Clamp01(volume);
            }
        }
        
                /// <summary>
                /// Get AudioClip by sound type
                /// </summary>
                private AudioClip GetClipByType(SoundType soundType)
                {
                    switch (soundType)
                    {
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
    }
}
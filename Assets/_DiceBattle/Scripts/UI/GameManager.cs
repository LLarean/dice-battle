using DiceBattle.Audio;
using DiceBattle.Core;
using DiceBattle.Data;
using DiceBattle.Screens;
using GameSignals;
using UnityEngine;

namespace DiceBattle.UI
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameConfig _config;
        [Header("UI References")]
        [SerializeField] private GameScreen _gameScreen;
        [SerializeField] private GameOverScreen _gameOverScreen;
        [Space]
        [SerializeField] private UnitPanel _enemy;

        private Enemy _currentEnemy;
        private int _currentHealth;
        private int _currentDefense;
        private int _enemiesDefeated;

        private bool _isFirstRoll;
        private bool _isGameOver;

        private int _attemptsIndex;

        #region Lifesycle

        private void Start()
        {
            _gameScreen.OnContextClicked += ContextAction;
            _gameOverScreen.OnRestartClicked += OnRestartButtonClicked;
            InitializeGame();
        }

        private void OnDestroy()
        {
            _gameScreen.OnContextClicked -= ContextAction;
            _gameOverScreen.OnRestartClicked -= OnRestartButtonClicked;
        }

        #endregion

        #region Button Actions

        private void ContextAction()
        {
            if (_attemptsIndex < _config.MaxAttempts - 1)
            {
                _gameScreen.RollUnlockedDice();
                
                _isFirstRoll = false;
                SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.DiceRoll));
            }
            else
            {
                _gameScreen.ShowAttempts(_config.MaxAttempts - _attemptsIndex);
                EndTurn();
            }
            
            _attemptsIndex++;

            if (_attemptsIndex < _config.MaxAttempts - 1)
            {
                _gameScreen.EnableDiceInteractable();
            }
            else
            {
                _gameScreen.DisableDiceInteractable();
                _gameScreen.SetContextLabel("End Turn");
            }
        }

        private void OnRestartButtonClicked() => InitializeGame();

        #endregion

        private void InitializeGame()
        {
            _currentHealth = _config.PlayerStartHealth;
            _currentDefense = 0;
            _enemiesDefeated = 0;
            _isFirstRoll = true;
            _isGameOver = false;
            _attemptsIndex = 0;

            _gameScreen.Initialize(_config.PlayerStartHealth);
            _gameOverScreen.gameObject.SetActive(false);
            SpawnEnemy();
        }
        
        private void SpawnEnemy()
        {
            _enemiesDefeated++;

            var portrait = _config.EnemiesPortraits[_enemiesDefeated - 1];
            
            _currentEnemy = Enemy.Create(_enemiesDefeated, portrait);
            _enemy.ShowEnemy(_currentEnemy);

            // TODO: SignalSystem.Raise - new enemy appearance
        }

        private void EndTurn()
        {
            // Calculate roll results
            int attack = 0;
            int defense = 0;
            int heal = 0;

            foreach (var dice in _gameScreen.Dices)
            {
                switch (dice.DiceType)
                {
                    case DiceType.Attack:
                        attack++;
                        break;
                    case DiceType.Defense:
                        defense++;
                        break;
                    case DiceType.Heal:
                        heal++;
                        break;
                }
            }

            // Apply healing
            if (heal > 0)
            {
                ApplyHealing(heal);
            }

            // Save defense for enemy's turn
            _currentDefense = defense;
            _gameScreen.UpdatePlayerDefense(_currentDefense);

            // Attack enemy
            if (attack > 0 && _currentEnemy != null)
            {
                int damageDealt = _currentEnemy.TakeDamage(attack);
                _enemy.UpdateDisplay();
                
                // TODO: SignalSystem.Raise - player attack (damage: damageDealt)
                
                if (!_currentEnemy.IsAlive)
                {
                    OnEnemyDefeated();
                    return;
                }
            }

            EnemyTurn();

            _isFirstRoll = true;
            UpdateButtonStates();
        }

        private void ApplyHealing(int heal)
        {
            _currentHealth = Mathf.Min(_config.PlayerStartHealth, _currentHealth + heal);
            _gameScreen.UpdateHealth(_currentHealth);

            // TODO: SignalSystem.Raise - healing (amount: heal)
        }

        private void EnemyTurn()
        {
            if (_currentEnemy == null || !_currentEnemy.IsAlive)
                return;

            int enemyAttack = _currentEnemy.PerformAttack();
            int damageToPlayer = Mathf.Max(0, enemyAttack - _currentDefense);

            if (damageToPlayer > 0)
            {
                _currentHealth = Mathf.Max(0, _currentHealth - damageToPlayer);
                _gameScreen.UpdateHealth(_currentHealth);
                
                // TODO: SignalSystem.Raise - player took damage (amount: damageToPlayer)

                if (_currentHealth <= 0)
                {
                    OnPlayerDefeated();
                    return;
                }
            }

            _currentDefense = 0;
            _gameScreen.UpdatePlayerDefense(0);
        }

        /// <summary>
        /// Enemy defeated
        /// </summary>
        private void OnEnemyDefeated()
        {
            // TODO: SignalSystem.Raise - enemy defeated
            
            // Spawn next enemy
            SpawnEnemy();

            // Start new turn
            _isFirstRoll = true;
            _currentDefense = 0;
            _gameScreen.UpdatePlayerDefense(0);
            UpdateButtonStates();
        }

        /// <summary>
        /// Player defeated (Game Over)
        /// </summary>
        private void OnPlayerDefeated()
        {
            _isGameOver = true;
            
            // TODO: SignalSystem.Raise - Game Over
            
            _gameOverScreen.Show(_enemiesDefeated - 1);
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            if (_isGameOver)
            {
                // _gameScreen.SetContextLabel("Action");
                _gameScreen.EnableDiceInteractable();
                return;
            }

            _gameScreen.SetContextLabel("Roll All");
        }
    }
}
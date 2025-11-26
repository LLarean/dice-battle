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
        [Header("Config")]
        [SerializeField] private GameConfig _config;

        [Header("UI References")]
        [SerializeField] private GameScreen _gameScreen;
        [SerializeField] private GameOverScreen _gameOverScreen;
        [Space]
        [SerializeField] private UnitPanel _enemy;
        [SerializeField] private DicePanel _dicePanel;

        // Game state
        private Enemy _currentEnemy;
        private int _playerHP;
        private int _playerMaxHP;
        private int _currentDefense; // Player's defense for this turn
        private int _enemiesDefeated;
        private bool _isFirstRoll; // First roll or second (reroll)
        private bool _isGameOver;
        
        private int _maxAttempts = 3;
        private int _attemptsCount;

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
        
        private void ContextAction()
        {
            if (_attemptsCount < _maxAttempts)
            {
                _dicePanel.RollUnlockedDice();
                
                // _dicePanel.EnableInteractable();
                
                _isFirstRoll = false;
                SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.DiceRoll));
            }

            _attemptsCount++;

            if (_attemptsCount < _maxAttempts)
            {
                _dicePanel.EnableInteractable();
            }
            else
            {
                _dicePanel.DisableInteractable();
            }
            

            if (_attemptsCount == _maxAttempts)
            {
                _dicePanel.ShowAttempts(_maxAttempts - _attemptsCount);
                EndPlayerTurn();
            }
        }

        /// <summary>
        /// Initialize a new game
        /// </summary>
        private void InitializeGame()
        {
            _dicePanel.Initialize();

            // Initialize player
            _playerMaxHP = _config.PlayerStartHP;
            _playerHP = _playerMaxHP;
            _currentDefense = 0;
            _enemiesDefeated = 0;
            _isFirstRoll = true;
            _isGameOver = false;

            // Initialize UI
            _gameScreen.Initialize(_playerMaxHP);
            _gameScreen.UpdateHealth(_playerHP);
            _gameScreen.UpdatePlayerDefense(0);
            
            _gameOverScreen.gameObject.SetActive(false);
            
            // Create first enemy
            SpawnNextEnemy();

            // Configure button UI
            UpdateButtonStates();
        }

        /// <summary>
        /// Spawn next enemy
        /// </summary>
        private void SpawnNextEnemy()
        {
            _enemiesDefeated++;

            var portrait = _config.EnemiesPortraits[_enemiesDefeated - 1];
            
            _currentEnemy = Enemy.Create(_enemiesDefeated, portrait);
            _enemy.ShowEnemy(_currentEnemy);

            // TODO: SignalSystem.Raise - new enemy appearance
        }

        /// <summary>
        /// "Roll Dice" / "End Turn" button
        /// </summary>
        private void OnRollButtonClicked()
        {

        }

        /// <summary>
        /// "Reroll" button
        /// </summary>
        private void OnRerollButtonClicked()
        {
            if (_isGameOver || _isFirstRoll) return;

            if(_attemptsCount >= _maxAttempts) return;
            
            RerollUnlockedDice();
            _attemptsCount++;
            
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.DiceReroll));
        }

        /// <summary>
        /// Roll all dice
        /// </summary>
        private void RollAllDice()
        {
            _dicePanel.RollAllDice();
        }

        /// <summary>
        /// Reroll unlocked dice
        /// </summary>
        private void RerollUnlockedDice()
        {
            
        }

        /// <summary>
        /// End player's turn (apply roll results)
        /// </summary>
        private void EndPlayerTurn()
        {
            // Calculate roll results
            int attack = 0;
            int defense = 0;
            int heal = 0;

            foreach (var dice in _dicePanel.Dices)
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
                    return; // Don't let enemy attack
                }
            }

            // Enemy's turn
            EnemyTurn();

            // Start new player turn
            _isFirstRoll = true;
            UpdateButtonStates();
        }

        private void ApplyHealing(int heal)
        {
            _playerHP = Mathf.Min(_playerMaxHP, _playerHP + heal);
            _gameScreen.UpdateHealth(_playerHP);

            // TODO: SignalSystem.Raise - healing (amount: heal)
        }

        /// <summary>
        /// Enemy's turn (attack)
        /// </summary>
        private void EnemyTurn()
        {
            if (_currentEnemy == null || !_currentEnemy.IsAlive)
                return;

            int enemyAttack = _currentEnemy.PerformAttack();
            int damageToPlayer = Mathf.Max(0, enemyAttack - _currentDefense);

            if (damageToPlayer > 0)
            {
                _playerHP = Mathf.Max(0, _playerHP - damageToPlayer);
                _gameScreen.UpdateHealth(_playerHP);
                
                // TODO: SignalSystem.Raise - player took damage (amount: damageToPlayer)

                if (_playerHP <= 0)
                {
                    OnPlayerDefeated();
                    return;
                }
            }

            // Reset defense after turn
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
            SpawnNextEnemy();

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
                _gameScreen.SetContextLabel("Action");
                _dicePanel.EnableInteractable();
                return;
            }

            _gameScreen.SetContextLabel("Roll All");
        }

        private void OnRestartButtonClicked() => InitializeGame();
    }
}
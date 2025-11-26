using UnityEngine;
using System.Collections.Generic;
using DiceBattle.Audio;
using DiceBattle.Core;
using DiceBattle.Data;
using DiceBattle.Screens;
using GameSignals;

namespace DiceBattle.UI
{
    /// <summary>
    /// Main game controller
    /// Manages all battle logic
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private GameConfig _config;

        [Header("UI References")]
        [SerializeField] private GameScreen _gameScreen;
        [SerializeField] private UnitPanel _enemy;
        [SerializeField] private DicePanel _dicePanel;

        // Game state
        private List<Dice> _dices;
        private Enemy _currentEnemy;
        private int _playerHP;
        private int _playerMaxHP;
        private int _currentDefense; // Player's defense for this turn
        private int _enemiesDefeated;
        private bool _isFirstRoll; // First roll or second (reroll)
        private bool _isGameOver;

        private void Start()
        {
            if (_config == null)
            {
                Debug.LogError("GameConfig is not assigned!");
                return;
            }

            InitializeGame();
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
            _gameScreen.UpdatePlayerHP(_playerHP);
            _gameScreen.UpdatePlayerDefense(0);
            _gameScreen.HideGameOver();
            _gameScreen.SubscribeToButtons(OnRollButtonClicked, OnRerollButtonClicked, OnRestartButtonClicked);

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
            if (_isGameOver)
                return;

            if (_isFirstRoll)
            {
                // First roll - roll all dice
                RollAllDice();
                _isFirstRoll = false;
                UpdateButtonStates();
                
                SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.DiceRoll));
            }
            else
            {
                // Second click - end player's turn
                EndPlayerTurn();
            }
        }

        /// <summary>
        /// "Reroll" button
        /// </summary>
        private void OnRerollButtonClicked()
        {
            if (_isGameOver || _isFirstRoll)
                return;

            RerollUnlockedDice();
            
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.DiceReroll));
        }

        /// <summary>
        /// Roll all dice
        /// </summary>
        private void RollAllDice()
        {
            _dicePanel.RollAllDice();
            _dicePanel.UpdateDiceVisuals();
        }

        /// <summary>
        /// Reroll unlocked dice
        /// </summary>
        private void RerollUnlockedDice()
        {
            _dicePanel.RerollUnlockedDice();
            _dicePanel.UpdateDiceVisuals();
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
                switch (dice.CurrentType)
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
                _playerHP = Mathf.Min(_playerMaxHP, _playerHP + heal);
                _gameScreen.UpdatePlayerHP(_playerHP);
                
                // TODO: SignalSystem.Raise - healing (amount: heal)
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
                _gameScreen.UpdatePlayerHP(_playerHP);
                
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
            
            _gameScreen.ShowGameOver(_enemiesDefeated - 1); // -1 because current enemy is not defeated
            UpdateButtonStates();
        }

        /// <summary>
        /// Update button states
        /// </summary>
        private void UpdateButtonStates()
        {
            if (_isGameOver)
            {
                _gameScreen.SetRollButtonState(true, false);
                _gameScreen.SetRerollButtonState(false, false);
                
                // foreach (var diceUI in _diceUIList)
                // {
                //     diceUI.SetInteractable(false);
                // }

                _dicePanel.EnableInteractable();
                return;
            }

            // Roll/Finish Turn button
            _gameScreen.SetRollButtonState(_isFirstRoll, true);

            // Reroll button (shown only after first roll)
            _gameScreen.SetRerollButtonState(!_isFirstRoll, true);
        }

        /// <summary>
        /// "Restart" button
        /// </summary>
        private void OnRestartButtonClicked()
        {
            InitializeGame();
        }
    }
}
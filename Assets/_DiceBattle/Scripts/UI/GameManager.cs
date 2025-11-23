using UnityEngine;
using System.Collections.Generic;
using DiceBattle.Audio;
using DiceBattle.Core;
using DiceBattle.Data;
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
        [SerializeField] private GameUI _gameUI;
        [SerializeField] private EnemyUI _enemyUI;
        [SerializeField] private UnitPanel _enemy;
        [SerializeField] private List<DiceUI> _diceUIList;

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
            // Create dice
            _dices = new List<Dice>();
            for (int i = 0; i < _config.DiceCount; i++)
            {
                _dices.Add(new Dice());
            }

            // Initialize dice UI
            for (int i = 0; i < _diceUIList.Count && i < _dices.Count; i++)
            {
                _diceUIList[i].Initialize(_dices[i], _config.DiceSprites);
            }

            // Initialize player
            _playerMaxHP = _config.PlayerStartHP;
            _playerHP = _playerMaxHP;
            _currentDefense = 0;
            _enemiesDefeated = 0;
            _isFirstRoll = true;
            _isGameOver = false;

            // Initialize UI
            _gameUI.Initialize(_playerMaxHP);
            _gameUI.UpdatePlayerHP(_playerHP);
            _gameUI.UpdatePlayerDefense(0);
            _gameUI.HideGameOver();
            _gameUI.SubscribeToButtons(OnRollButtonClicked, OnRerollButtonClicked, OnRestartButtonClicked);

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
            _enemyUI.ShowEnemy(_currentEnemy);
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
            foreach (var dice in _dices)
            {
                dice.Unlock();
                dice.Roll();
            }

            UpdateDiceVisuals();
        }

        /// <summary>
        /// Reroll unlocked dice
        /// </summary>
        private void RerollUnlockedDice()
        {
            foreach (var dice in _dices)
            {
                if (!dice.IsLocked)
                {
                    dice.Roll();
                }
            }

            UpdateDiceVisuals();
        }

        /// <summary>
        /// Update visual of all dice
        /// </summary>
        private void UpdateDiceVisuals()
        {
            for (int i = 0; i < _diceUIList.Count && i < _dices.Count; i++)
            {
                _diceUIList[i].UpdateVisual();
            }
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

            foreach (var dice in _dices)
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
                _gameUI.UpdatePlayerHP(_playerHP);
                
                // TODO: SignalSystem.Raise - healing (amount: heal)
            }

            // Save defense for enemy's turn
            _currentDefense = defense;
            _gameUI.UpdatePlayerDefense(_currentDefense);

            // Attack enemy
            if (attack > 0 && _currentEnemy != null)
            {
                int damageDealt = _currentEnemy.TakeDamage(attack);
                _enemyUI.UpdateDisplay();
                
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
                _gameUI.UpdatePlayerHP(_playerHP);
                
                // TODO: SignalSystem.Raise - player took damage (amount: damageToPlayer)

                if (_playerHP <= 0)
                {
                    OnPlayerDefeated();
                    return;
                }
            }

            // Reset defense after turn
            _currentDefense = 0;
            _gameUI.UpdatePlayerDefense(0);
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
            _gameUI.UpdatePlayerDefense(0);
            UpdateButtonStates();
        }

        /// <summary>
        /// Player defeated (Game Over)
        /// </summary>
        private void OnPlayerDefeated()
        {
            _isGameOver = true;
            
            // TODO: SignalSystem.Raise - Game Over
            
            _gameUI.ShowGameOver(_enemiesDefeated - 1); // -1 because current enemy is not defeated
            UpdateButtonStates();
        }

        /// <summary>
        /// Update button states
        /// </summary>
        private void UpdateButtonStates()
        {
            if (_isGameOver)
            {
                _gameUI.SetRollButtonState(true, false);
                _gameUI.SetRerollButtonState(false, false);
                
                foreach (var diceUI in _diceUIList)
                {
                    diceUI.SetInteractable(false);
                }
                return;
            }

            // Roll/Finish Turn button
            _gameUI.SetRollButtonState(_isFirstRoll, true);

            // Reroll button (shown only after first roll)
            _gameUI.SetRerollButtonState(!_isFirstRoll, true);

            // Dice are interactive only after first roll
            foreach (var diceUI in _diceUIList)
            {
                diceUI.SetInteractable(!_isFirstRoll);
            }
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
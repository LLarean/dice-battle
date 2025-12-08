using System.Collections.Generic;
using DiceBattle.Audio;
using DiceBattle.Core;
using DiceBattle.Data;
using DiceBattle.Screens;
using GameSignals;
using UnityEngine;

namespace DiceBattle.UI
{
    public class GameLoop : MonoBehaviour
    {
        [SerializeField] private GameConfig _config;
        [SerializeField] private DiceRollAnimation _diceRollAnimation;
        [Header("UI References")]
        [SerializeField] private GameScreen _gameScreen;
        [SerializeField] private GameOverScreen _gameOverScreen;
        [SerializeField] private LootScreen _lootScreen;
        [Space]
        [SerializeField] private UnitPanel _enemy;

        private Enemy _currentEnemy;
        private int _currentHealth;
        private int _currentDefense;
        private int _enemiesDefeated;

        private TurnResult _turnResult = new();

        private bool _isFirstRoll;
        private bool _isGameOver;

        private int _attemptsNumber;

        private void InitializeGame()
        {
            _currentHealth = _config.PlayerStartHealth;
            _currentDefense = 0;
            _enemiesDefeated = 0;
            _isFirstRoll = true;
            _isGameOver = false;
            _attemptsNumber = 0;

            _gameScreen.Initialize(_config.PlayerStartHealth, _config.EnemyBaseHealth);
            _gameScreen.DisableDiceInteractable();

            _gameOverScreen.gameObject.SetActive(false);
            SpawnEnemy();
        }

        private void SpawnEnemy()
        {
            Sprite portrait = _config.EnemiesPortraits[_enemiesDefeated];
            _currentEnemy = Enemy.Create(_enemiesDefeated, portrait);

            var unitData = new UnitData
            {
                Title = $"Enemy #{_currentEnemy.Number}",
                Portrait = _config.EnemiesPortraits[_enemiesDefeated],
                HealthMax = _currentEnemy.MaxHP,
                HealthCurrent = _currentEnemy.MaxHP,
                Attack = _currentEnemy.Attack,
                Defense = _currentEnemy.Defense,
            };

            Debug.Log("unitData.Title = " + unitData.Title);
            Debug.Log("unitData.HealthMax = " + unitData.HealthMax);
            Debug.Log("unitData.HealthCurrent = " + unitData.HealthCurrent);
            Debug.Log("unitData.Attack = " + unitData.Attack);
            Debug.Log("unitData.Defense = " + unitData.Defense);

            _gameScreen.SetEnemyData(unitData);

            // _enemy.ShowEnemy(_currentEnemy);
            _enemiesDefeated++;

            // TODO: SignalSystem.Raise - new enemy appearance
        }

        private void EndTurn()
        {
            Debug.Log("EndTurn");
            _turnResult.Calculate(_gameScreen.Dices);

            if (_turnResult.Heal > 0)
            {
                ApplyHealing(_turnResult.Heal);
            }

            // Save defense for enemy's turn
            _currentDefense = _turnResult.Defense;
            _gameScreen.UpdatePlayerDefense(_currentDefense);

            // Attack enemy
            if (_turnResult.Attack > 0)
            {
                int damageDealt = _currentEnemy.TakeDamage(_turnResult.Attack);
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
            _attemptsNumber = 0;

            _gameScreen.ResetSelection();
            _gameScreen.SetContextLabel("Roll All");

            UpdateButtonStates();
        }

        private void ApplyHealing(int heal)
        {
            _currentHealth = Mathf.Min(_config.PlayerStartHealth, _currentHealth + heal);
            _gameScreen.UpdatePlayerHealth(_currentHealth);

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
                _gameScreen.UpdatePlayerHealth(_currentHealth);

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
            // TODO: SignalSystem.Raise - The sound of victory/defeat

            _lootScreen.gameObject.SetActive(true);

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
            // if (_isGameOver)
            // {
            //     // _gameScreen.SetContextLabel("Action");
            //     _gameScreen.EnableDiceInteractable();
            //     return;
            // }

            if (_attemptsNumber == 0)
            {
                _gameScreen.DisableDiceInteractable();
            }
            else if (_attemptsNumber >= _config.MaxAttempts - 1)
            {
                _gameScreen.DisableDiceInteractable();
                _gameScreen.SetContextLabel("End Turn");
            }
            else
            {
                _gameScreen.EnableDiceInteractable();
            }
        }

        #region Event Handlers

        private void HandleContextAction()
        {
            _attemptsNumber++;

            if (_isFirstRoll)
            {
                _isFirstRoll = false;
                _gameScreen.RollDice();

                SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.DiceRoll));
            }
            else
            {
                if (_attemptsNumber < _config.MaxAttempts)
                {
                    _gameScreen.RerollSelectedDice();

                    _isFirstRoll = false;
                    SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.DiceRoll));
                }
                else
                {
                    EndTurn();
                }
            }

            Debug.Log($"Attempts = {_attemptsNumber}");

            UpdateButtonStates();
            // TODO: SignalSystem.Raise - The button is clicked (click sound)
        }

        private void HandleRestartButtonClicked()
        {
            // TODO: SignalSystem.Raise - The button is clicked (click sound)

            InitializeGame();
        }

        #endregion

        #region Unity lifecycle

        private void Start()
        {
            _gameScreen.OnContextClicked += HandleContextAction;
            _gameOverScreen.OnRestartClicked += HandleRestartButtonClicked;
            InitializeGame();
        }

        private void OnDestroy()
        {
            _gameScreen.OnContextClicked -= HandleContextAction;
            _gameOverScreen.OnRestartClicked -= HandleRestartButtonClicked;
        }

        #endregion
    }

    public class TurnResult
    {
        private int _attack;
        private int _defense;
        private int _heal;

        public int Attack => _attack;
        public int Defense => _defense;
        public int Heal => _heal;

        public void Calculate(List<Dice> dices)
        {
            _attack = 0;
            _defense = 0;
            _heal = 0;

            foreach (Dice dice in dices)
            {
                switch (dice.DiceType)
                {
                    case DiceType.Attack:
                        _attack++;
                        break;
                    case DiceType.Defense:
                        _defense++;
                        break;
                    case DiceType.Heal:
                        _heal++;
                        break;
                }
            }
        }
    }
}

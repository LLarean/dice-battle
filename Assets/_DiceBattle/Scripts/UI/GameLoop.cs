using DiceBattle.Audio;
using DiceBattle.Data;
using DiceBattle.Screens;
using GameSignals;
using UnityEngine;

namespace DiceBattle.UI
{
    public class GameLoop : MonoBehaviour
    {
        private readonly TurnResult _turnResult = new();

        [SerializeField] private GameConfig _config;
        [Space]
        [SerializeField] private GameScreen _gameScreen;
        [SerializeField] private GameOverScreen _gameOverScreen;
        [SerializeField] private LootScreen _lootScreen;

        private UnitData _playerData;
        private UnitData _enemyData;

        private int _enemiesDefeated;
        private int _attemptsNumber;
        private bool _isFirstRoll;

        public void InitializeGame()
        {
            _enemiesDefeated = 0;
            _isFirstRoll = true;
            _attemptsNumber = 0;

            SpawnHero();
            SpawnEnemy();

            _gameScreen.DisableDiceInteractable();
            _gameOverScreen.gameObject.SetActive(false);
        }

        private void SpawnHero()
        {
            _playerData = new UnitData
            {
                Title = "Hero (you)",
                Portrait = _config.PlayerPortrait,
                MaxHealth = _config.PlayerStartHealth,
                CurrentHealth = _config.PlayerStartHealth,
                Attack = 0,
                Defense = 0,
            };

            _playerData.Log();
            _gameScreen.SetPlayerData(_playerData);
        }

        private void SpawnEnemy()
        {
            int maxHealth = _config.EnemyBaseHealth + _config.EnemyHPGrowth * _enemiesDefeated;
            int attack = _config.EnemyBaseAttack + _config.EnemyAttackGrowthRate * _enemiesDefeated;
            int defense = _config.EnemyBaseDefense + _config.EnemyDefenseGrowthRate * _enemiesDefeated;

            _enemyData = new UnitData
            {
                Title = $"Enemy #{_enemiesDefeated + 1}",
                Portrait = _config.EnemiesPortraits[_enemiesDefeated],
                MaxHealth = maxHealth,
                CurrentHealth = maxHealth,
                Attack = attack,
                Defense = defense,
            };

            _enemyData.Log();
            _gameScreen.SetEnemyData(_enemyData);

            _enemiesDefeated++;

            // TODO: SignalSystem.Raise - new enemy appearance (new level)
        }

        private void EndTurn()
        {
            _turnResult.Calculate(_gameScreen.Dices);

            ApplyHealing();
            ApplyDefense();
            ApplyAttack();

            if (_enemyData.CurrentHealth <= 0)
            {
                OnEnemyDefeated();
            }
            else
            {
                EnemyTurn();
            }

            _isFirstRoll = true;
            _attemptsNumber = 0;

            _gameScreen.ResetSelection();
            _gameScreen.SetContextLabel("Roll All");

            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
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
                _gameScreen.SetContextLabel("Skip");
            }
        }

        #region Player actions

        private void ApplyHealing()
        {
            if (_turnResult.Heal <= 0)
            {
                return;
            }

            _playerData.CurrentHealth = Mathf.Min(_config.PlayerStartHealth, _playerData.CurrentHealth + _turnResult.Heal);
            _gameScreen.UpdatePlayerHealth(_playerData.CurrentHealth);

            // TODO: SignalSystem.Raise - healing (amount: heal)
        }

        private void ApplyAttack()
        {
            if (_turnResult.Attack <= 0)
            {
                return;
            }

            TakeDamage(_turnResult.Attack);
            _gameScreen.UpdateEnemyDisplay();

            // TODO: SignalSystem.Raise - player attack (damage: damageDealt)
        }

        private void ApplyDefense()
        {
            _playerData.Defense = _turnResult.Defense;
            _gameScreen.UpdatePlayerDefense(_playerData.Defense);
        }

        private void OnPlayerDefeated()
        {
            // TODO: SignalSystem.Raise - Game Over

            _gameOverScreen.Show(_enemiesDefeated - 1);
            UpdateButtonStates();
        }

        #endregion

        #region Enemy actions

        private void EnemyTurn()
        {
            int enemyAttack = _enemyData.Attack;
            int damageToPlayer = Mathf.Max(0, enemyAttack - _playerData.Defense);

            if (damageToPlayer > 0)
            {
                _playerData.CurrentHealth = Mathf.Max(0, _playerData.CurrentHealth - damageToPlayer);
                _gameScreen.UpdatePlayerHealth(_playerData.CurrentHealth);

                // TODO: SignalSystem.Raise - player took damage (amount: damageToPlayer)

                if (_playerData.CurrentHealth <= 0)
                {
                    OnPlayerDefeated();
                    return;
                }
            }

            _playerData.Defense = 0;
            _gameScreen.UpdatePlayerDefense(0);
        }

        private void OnEnemyDefeated()
        {
            // TODO: SignalSystem.Raise - The sound of victory/defeat

            _lootScreen.gameObject.SetActive(true);

            SpawnEnemy();

            _isFirstRoll = true;
            _playerData.Defense = 0;

            _gameScreen.UpdatePlayerDefense(0);

            UpdateButtonStates();
        }

        #endregion

        #region Event Handlers

        private void HandleContextAction()
        {
            _attemptsNumber++;

            if (_isFirstRoll)
            {
                _isFirstRoll = false;
                _gameScreen.RollDice();

                // SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.DiceRoll));
            }
            else
            {
                if (_attemptsNumber < _config.MaxAttempts)
                {
                    _gameScreen.RerollSelectedDice();

                    _isFirstRoll = false;
                    // SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.DiceRoll));
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

        private int TakeDamage(int damage)
        {
            int actualDamage = Mathf.Max(0, damage - _enemyData.Defense);
            _enemyData.CurrentHealth = Mathf.Max(0, _enemyData.CurrentHealth - actualDamage);

            // TODO: SignalSystem.Raise - The enemy has taken damage (actualDamage)

            return actualDamage;
        }

        #endregion

        #region Unity lifecycle

        private void Start()
        {
            _gameScreen.OnContextClicked += HandleContextAction;
            _gameOverScreen.OnRestartClicked += HandleRestartButtonClicked;
        }

        private void OnDestroy()
        {
            _gameScreen.OnContextClicked -= HandleContextAction;
            _gameOverScreen.OnRestartClicked -= HandleRestartButtonClicked;
        }

        #endregion
    }
}

using DiceBattle.Audio;
using DiceBattle.Data;
using DiceBattle.Events;
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

        private bool _isFirstRoll;
        private int _attemptsNumber;

        public void InitializeGame()
        {
            _isFirstRoll = true;
            _attemptsNumber = 0;

            SpawnHero();
            SpawnEnemy();
            // UpdateDiceCount();

            _gameScreen.DisableDiceInteractable();
            _gameOverScreen.gameObject.SetActive(false);
        }

        private void SpawnHero()
        {
            // _playerData = new UnitData();

            // _playerData.Create(_config);

            _playerData = new UnitData
            {
                Title = "Герой (вы)", // TODO Translation
                Portrait = _config.PlayerPortrait,
                MaxHealth = _config.PlayerStartHealth,
                CurrentHealth = _config.PlayerStartHealth,
                Attack = _config.PlayerStartDamage,
                Armor = _config.PlayerStartArmor,
            };

            _playerData.Log();
            _gameScreen.SetPlayerData(_playerData);
        }

        // TODO Refactoring is needed
        private void UpdateHero()
        {
            _playerData.Update(_config);
            _playerData.Log();
            _gameScreen.SetPlayerData(_playerData);
            // UpdateDiceCount(rewardTypes);
        }

        private void UpdateDiceCount()
        {
            int diceCount = GameProgress.GetDiceCount();
            _gameScreen.SetDiceCount(diceCount);
        }

        private void SpawnEnemy()
        {
            int maxHealth = _config.EnemyBaseHealth + _config.EnemyHPGrowth * GameProgress.CompletedLevels;
            int attack = _config.EnemyBaseAttack + _config.EnemyAttackGrowthRate * GameProgress.CompletedLevels;
            int defense = _config.EnemyBaseDefense + _config.EnemyDefenseGrowthRate * GameProgress.CompletedLevels;

            _enemyData = new UnitData
            {
                Title = $"Враг #{GameProgress.CompletedLevels + 1}", // TODO Translation
                Portrait = _config.EnemiesPortraits[GameProgress.CompletedLevels],
                MaxHealth = maxHealth,
                CurrentHealth = maxHealth,
                Attack = attack,
                Armor = defense,
            };

            _enemyData.Log();
            _gameScreen.SetEnemyData(_enemyData);

            GameProgress.IncrementLevels();
            // _enemiesDefeated++;

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Victory));
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
            _gameScreen.SetContextLabel("Бросить все"); // TODO Translation

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
                _gameScreen.SetContextLabel("Закончить"); // TODO Translation
            }
            else
            {
                _gameScreen.EnableDiceInteractable();
                _gameScreen.SetContextLabel("Пропустить"); // TODO Translation
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

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.PlayerHeal));
        }

        private void ApplyAttack()
        {
            if (_turnResult.Attack <= 0)
            {
                return;
            }

            TakeDamage(_turnResult.Attack);
            _gameScreen.UpdateEnemyDisplay();

            // SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.SlimeAttack));
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.EnemyHit));
        }

        private void ApplyDefense()
        {
            _playerData.Armor = _turnResult.Defense;
            _gameScreen.UpdatePlayerDefense(_playerData.Armor);
        }

        private void OnPlayerDefeated()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Defeat));
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.GameOverScreen));

            UpdateButtonStates();
        }

        #endregion

        #region Enemy actions

        private void EnemyTurn()
        {
            int enemyAttack = _enemyData.Attack;
            int damageToPlayer = Mathf.Max(0, enemyAttack - _playerData.Armor);

            if (damageToPlayer > 0)
            {
                _playerData.CurrentHealth = Mathf.Max(0, _playerData.CurrentHealth - damageToPlayer);
                _gameScreen.UpdatePlayerHealth(_playerData.CurrentHealth);

                SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.SlimeAttack));

                if (_playerData.CurrentHealth <= 0)
                {
                    OnPlayerDefeated();
                    return;
                }
            }

            _playerData.Armor = 0;
            _gameScreen.UpdatePlayerDefense(0);
        }

        private void OnEnemyDefeated()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.EnemyDefeated));

            _lootScreen.gameObject.SetActive(true);
            _lootScreen.RollReward();

            GameProgress.IncrementCurrentLevel();

            UpdateHero();
            SpawnEnemy();

            _isFirstRoll = true;
            _playerData.Armor = 0;

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
            }
            else
            {
                if (_attemptsNumber < _config.MaxAttempts)
                {
                    _gameScreen.RerollSelectedDice();

                    _isFirstRoll = false;
                }
                else
                {
                    EndTurn();
                }
            }

            UpdateButtonStates();
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
        }

        private void HandleRestartButtonClicked() => InitializeGame();

        private int TakeDamage(int damage)
        {
            int actualDamage = Mathf.Max(0, damage - _enemyData.Armor);
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
            _lootScreen.OnRewardSelected += UpdateHero;
        }

        private void OnDestroy()
        {
            _gameScreen.OnContextClicked -= HandleContextAction;
            _gameOverScreen.OnRestartClicked -= HandleRestartButtonClicked;
            _lootScreen.OnRewardSelected -= UpdateHero;
        }

        #endregion
    }
}

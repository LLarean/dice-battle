using System.Linq;
using DiceBattle.Audio;
using DiceBattle.Data;
using DiceBattle.Events;
using DiceBattle.Global;
using DiceBattle.UI;
using GameSignals;
using UnityEngine;

namespace DiceBattle.Core
{
    public class GameLoop : MonoBehaviour
    {
        private readonly DiceResult _diceResult = new();

        [SerializeField] private GameConfig _config;
        [Space]
        [SerializeField] private GameScreen _gameScreen;
        [SerializeField] private LootScreen _lootScreen;

        private UnitData _playerData;
        private UnitData _enemyData;
        private Rewards _rewards;

        private bool _isFirstRoll;
        private int _attemptsNumber;

        public void InitializeGame()
        {
            _isFirstRoll = true;
            _attemptsNumber = 0;

            SpawnEnemy();
            SpawnHero();
            UpdateDiceCount();

            _gameScreen.DisableDiceInteractable();
        }

        private void SpawnEnemy()
        {
            int maxHealth = _config.EnemyBaseHealth + _config.EnemyHPGrowth * GameProgress.CompletedLevels;
            int damage = _config.EnemyBaseAttack + _config.EnemyAttackGrowthRate * GameProgress.CompletedLevels;
            int armor = _config.EnemyBaseDefense + _config.EnemyDefenseGrowthRate * GameProgress.CompletedLevels;

            _enemyData = new UnitData
            {
                Title = $"Враг #{GameProgress.CompletedLevels + 1}", // TODO Translation
                Portrait = _config.EnemiesPortraits[GameProgress.CompletedLevels],
                MaxHealth = maxHealth,
                CurrentHealth = maxHealth,
                Attack = damage,
                Armor = armor,
            };

            _enemyData.Log();
            _gameScreen.SetEnemyData(_enemyData);
        }

        private void SpawnHero()
        {
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

        private void UpdateHero()
        {
            _playerData.Update(_config);
            _playerData.Log();
            _gameScreen.SetPlayerData(_playerData);
            UpdateDiceCount();
        }

        private void UpdateDiceCount()
        {
            // TODO Improvements in the number of cubes
            // int diceCount = GameProgress.GetDiceCount();
            _gameScreen.SetDiceCount(_config.DiceStartCount);
        }

        private void EndTurn()
        {
            _rewards = GameProgress.GetRewards();
            _diceResult.Calculate(_gameScreen.Dices);

            // ApplyDefense();
            ApplyAttack();
            ApplyHealing();

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
                _gameScreen.SetContextLabel("Закончить"); // TODO Translation
            }
        }

        #region Player actions

        private void ApplyDefense()
        {
            int bonusArmor = _rewards.RewardTypes.Count(r => r == RewardType.Armor) * _config.PlayerBonusArmor;

            _playerData.Armor = Mathf.Max(0, _diceResult.Armor + bonusArmor);
            _gameScreen.UpdatePlayerDefense(_playerData.Armor);
        }

        private void ApplyAttack()
        {
            int doubleDamageCount = _rewards.RewardTypes.Count(r => r == RewardType.DoubleDamage);
            // int doubleDamage = _rewards.RewardTypes.Where(rewardType => rewardType == RewardType.DoubleDamage).Sum(rewardType => 1);

            Debug.Log("doubleDamage = " + doubleDamageCount);

            int damageToEnemy = Mathf.Max(_diceResult.Damage, _diceResult.Damage * doubleDamageCount);

            Debug.Log("damageToEnemy = " + damageToEnemy);

            EnemyTakeDamage(damageToEnemy);
            _gameScreen.UpdateEnemyDisplay();

            // TODO You can add different sounds to attack different enemies
            // SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.SlimeAttack));
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.EnemyHit));
        }

        private void ApplyHealing()
        {
            int doubleHealth = _rewards.RewardTypes.Count(r => r == RewardType.DoubleHealth);
            int regenHealth = _rewards.RewardTypes.Count(r => r == RewardType.RegenHealth);

            // int doubleHealth = _rewards.RewardTypes.Where(rewardType => rewardType == RewardType.DoubleHealth).Sum(rewardType => 1);
            // int regenHealth = _rewards.RewardTypes.Where(rewardType => rewardType == RewardType.RegenHealth).Sum(rewardType => 1);

            int fullHealth = Mathf.Max(_config.PlayerStartHealth, _config.PlayerStartHealth * doubleHealth);

            // int fullHealth = _config.PlayerStartHealth * doubleHealth;
            int currentHealth = _playerData.CurrentHealth + _diceResult.Heal + regenHealth;

            _playerData.CurrentHealth = Mathf.Min(fullHealth, currentHealth);
            _gameScreen.UpdatePlayerHealth(_playerData.CurrentHealth);

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.PlayerHeal));
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

            Debug.Log("damageToPlayer = " + damageToPlayer);

            if (damageToPlayer > 0)
            {
                _playerData.CurrentHealth = Mathf.Max(0, _playerData.CurrentHealth - damageToPlayer);

                Debug.Log("Player health = " + _playerData.CurrentHealth);

                _gameScreen.UpdatePlayerHealth(_playerData.CurrentHealth);

                SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.SlimeAttack));

                if (_playerData.CurrentHealth <= 0)
                {
                    Debug.Log("Player health = 0");
                    OnPlayerDefeated();
                    return;
                }
            }

            int bonusArmor = _rewards.RewardTypes.Where(rewardType => rewardType == RewardType.Armor).Sum(rewardType => 1);
            _playerData.Armor = bonusArmor;
            _gameScreen.UpdatePlayerDefense(bonusArmor);
        }

        private void OnEnemyDefeated()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.EnemyDefeated));

            _lootScreen.gameObject.SetActive(true);
            _lootScreen.RollReward();

            GameProgress.IncrementCurrentLevel();

            UpdateHero();
            SpawnEnemy();

            GameProgress.IncrementLevels();
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Victory));

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

        private int EnemyTakeDamage(int damage)
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
            _lootScreen.OnRewardSelected += UpdateHero;
        }

        private void OnDestroy()
        {
            _gameScreen.OnContextClicked -= HandleContextAction;
            _lootScreen.OnRewardSelected -= UpdateHero;
        }

        #endregion
    }
}

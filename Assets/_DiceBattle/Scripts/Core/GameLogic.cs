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
    public class GameLogic
    {
        private readonly DiceResult _diceResult = new();
        private readonly Spawner _spawner;

        private GameConfig _config;
        private GameScreen _gameScreen;

        private UnitData _playerData;
        private UnitData _enemyData;
        private Rewards _rewards;

        private bool _isFirstRoll;
        private int _attemptsNumber;

        public GameLogic(GameConfig config, GameScreen gameScreen)
        {
            _config = config;
            _gameScreen = gameScreen;

            _spawner = new Spawner(config, gameScreen);
        }

        public void InitializeGame()
        {
            _isFirstRoll = true;
            _attemptsNumber = 0;

            SpawnEnemy();
            SpawnHero();
            UpdateDiceCount();

            _gameScreen.DisableDiceInteractable();
        }

        public void ContextClick()
        {
            _attemptsNumber++;
            HandleContextAction();
        }

        private void EndTurn()
        {
            _rewards = GameProgress.GetReceivedRewards();
            _diceResult.Calculate(_gameScreen.Dices);

            ApplyArmor();
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

        #region Spawns

        private void SpawnEnemy()
        {
            _enemyData = _spawner.SpawnEnemy();
        }

        private void SpawnHero()
        {
            _playerData = _spawner.SpawnHero();
        }

        #endregion

        #region Updates

        public void UpdateHero()
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

        #endregion

        #region Player actions

        private void ApplyArmor()
        {
            int bonusArmor = _rewards.RewardTypes.Count(r => r == RewardType.Armor) * _config.Player.GrowthArmor;
            _playerData.Armor = Mathf.Max(0, _diceResult.Armor + bonusArmor);
            _gameScreen.UpdatePlayerArmor(_playerData.Armor);
        }

        private void RemoveArmor()
        {
            int bonusArmor = _rewards.RewardTypes.Count(r => r == RewardType.Armor) * _config.Player.GrowthArmor;
            _playerData.Armor = Mathf.Max(0, bonusArmor);
            _gameScreen.UpdatePlayerArmor(bonusArmor);
        }

        private void ApplyAttack()
        {
            int doubleDamageCount = _rewards.RewardTypes.Count(r => r == RewardType.DoubleDamage);
            int damageToEnemy = Mathf.Max(_diceResult.Damage, _diceResult.Damage * doubleDamageCount);

            EnemyTakeDamage(damageToEnemy);
            _gameScreen.UpdateEnemyDisplay();

            // TODO You can add different sounds to attack different enemies
            // SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.SlimeAttack));
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.EnemyHit));
        }

        private void ApplyHealing()
        {
            int rewardRegenHealth = _rewards.RewardTypes.Count(r => r == RewardType.RegenHealth) * _config.Player.RegenHealth;
            int allRegenHealth = _diceResult.Heal + rewardRegenHealth;
            _playerData.CurrentHealth = Mathf.Min(_playerData.MaxHealth, _playerData.CurrentHealth + allRegenHealth);
            _gameScreen.UpdatePlayerHealth(_playerData.CurrentHealth);

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.PlayerHeal));
        }

        private void OnPlayerDefeated()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Defeat));
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.GameOverScreen));
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

            RemoveArmor();
        }

        private void OnEnemyDefeated()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.EnemyDefeated));
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.LootScreen));

            GameProgress.IncrementCurrentLevel();

            UpdateHero();
            SpawnEnemy();

            GameProgress.IncrementLevels();
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Victory));

            _isFirstRoll = true;
            _playerData.Armor = 0;

            _gameScreen.UpdatePlayerArmor(0);

            UpdateButtonStates();
        }

        #endregion

        #region Event Handlers

        private void HandleContextAction()
        {
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
        }

        private int EnemyTakeDamage(int damage)
        {
            int actualDamage = Mathf.Max(0, damage - _enemyData.Armor);
            _enemyData.CurrentHealth = Mathf.Max(0, _enemyData.CurrentHealth - actualDamage);

            // TODO: SignalSystem.Raise - The enemy has taken damage (actualDamage)

            return actualDamage;
        }

        #endregion
    }
}

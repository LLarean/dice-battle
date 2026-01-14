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
        private readonly GameConfig _config;
        private readonly GameScreen _gameScreen;
        private readonly Spawner _spawner;
        private readonly DiceResult _diceResult = new();

        private UnitData _playerData;
        private UnitData _enemyData;

        private Rewards _rewards;
        private int _attemptsNumber;

        public GameLogic(GameConfig config, GameScreen gameScreen)
        {
            _config = config;
            _gameScreen = gameScreen;

            _spawner = new Spawner(config, gameScreen);
        }

        public void InitializeGame()
        {
            _attemptsNumber = 0;

            _enemyData = _spawner.SpawnEnemy();
            _playerData = _spawner.SpawnHero();

            UpdateDiceCount();
            _gameScreen.DisableDiceInteractable();
        }

        public void ContextClick()
        {
            _attemptsNumber++;

            if (_attemptsNumber == 1)
            {
                _gameScreen.RollDice();
            }
            else if (_attemptsNumber < _config.MaxAttempts)
            {
                if (_gameScreen.HaveSelectedDice)
                {
                    _gameScreen.RerollSelectedDice();
                }
                else
                {
                    EndTurn();
                }
            }
            else
            {
                EndTurn();
            }

            UpdateButtonStates();
        }

        private void EndTurn()
        {
            _rewards = GameProgress.GetReceivedRewards();
            _diceResult.Calculate(_gameScreen.Dices);

            ApplyPlayerArmor();
            ApplyPlayerAttack();
            ApplyHealing();

            if (_enemyData.CurrentHealth <= 0)
            {
                OnEnemyDefeated();
            }
            else
            {
                EnemyTurn();
            }

            _attemptsNumber = 0;
            _gameScreen.ResetSelection();
            _gameScreen.SetContextLabel("Бросить все"); // TODO Translation

            UpdateButtonStates();
        }

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

        private void ApplyPlayerArmor()
        {
            int bonusArmor = _rewards.RewardTypes.Count(r => r == RewardType.Armor) * _config.Player.GrowthArmor;
            _playerData.Armor = Mathf.Max(0, _diceResult.Armor + bonusArmor);
            _gameScreen.UpdatePlayerArmor(_playerData.Armor);
        }

        private void RemovePlayerArmor()
        {
            int bonusArmor = _rewards.RewardTypes.Count(r => r == RewardType.Armor) * _config.Player.GrowthArmor;
            _playerData.Armor = Mathf.Max(0, bonusArmor);
            _gameScreen.UpdatePlayerArmor(bonusArmor);
        }

        private void ApplyPlayerAttack()
        {
            int doubleDamageCount = _rewards.RewardTypes.Count(r => r == RewardType.DoubleDamage);
            int damageToEnemy = Mathf.Max(_diceResult.Damage, _diceResult.Damage * doubleDamageCount);

            _gameScreen.EnemyTakeDamage(damageToEnemy);

            // EnemyTakeDamage(damageToEnemy);
            _gameScreen.UpdateEnemyDisplay();

            // TODO You can add different sounds to attack different enemies
            // SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.SlimeAttack));
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.EnemyHit));
        }

        private void ApplyHealing()
        {
            int rewardRegenHealth = _rewards.RewardTypes.Count(r => r == RewardType.RegenHealth) * _config.Player.RegenHealth;
            int allRegenHealth = _diceResult.Heal + rewardRegenHealth;
            // _playerData.CurrentHealth = Mathf.Min(_playerData.MaxHealth, _playerData.CurrentHealth + allRegenHealth);

            _gameScreen.PlayerTakeHeal(allRegenHealth);
            // _gameScreen.UpdatePlayerHealth(_playerData.CurrentHealth);

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
            _gameScreen.PlayerTakeDamage(_enemyData.Damage);

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.SlimeAttack));

            if (_playerData.CurrentHealth <= 0)
            {
                OnPlayerDefeated();
                return;
            }

            RemovePlayerArmor();
        }

        private void OnEnemyDefeated()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.EnemyDefeated));
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.LootScreen));

            GameProgress.IncrementCurrentLevel();

            UpdateHero();
            _enemyData = _spawner.SpawnEnemy();

            GameProgress.IncrementLevels();
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Victory));

            // TODO Add bonus armor
            _playerData.Armor = 0;
            _gameScreen.UpdatePlayerArmor(0);

            UpdateButtonStates();
        }

        #endregion
    }
}

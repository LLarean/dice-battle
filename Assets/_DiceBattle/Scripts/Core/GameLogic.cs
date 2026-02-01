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

        private RewardsData _rewardsData;
        private int _attemptsNumber;
        private int _maxAttempts;

        private int _playerHealthDelta;
        private int _enemyHealthDelta;

        public GameLogic(GameConfig config, GameScreen gameScreen)
        {
            _config = config;
            _gameScreen = gameScreen;
            _spawner = new Spawner(config, gameScreen);
        }

        public void InitializeGame()
        {
            ResetNumbers();
            UpdateDiceCount();

            _enemyData = _spawner.SpawnEnemy();
            _playerData = _spawner.SpawnHero();

            _gameScreen.DisableDiceInteractable();
            SignalSystem.Raise<IHintHandler>(handler => handler.Hide());
        }

        public void ContextClick()
        {
            _attemptsNumber++;

            if (_attemptsNumber == 1)
            {
                _gameScreen.RollDice();
            }
            else if (_attemptsNumber < _maxAttempts)
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

        public void AllClick()
        {
            if (_attemptsNumber <= 0)
            {
                return;
            }

            if (_attemptsNumber >= _maxAttempts)
            {
                return;
            }

            _gameScreen.ToggleAllDice();
        }

        private void EndTurn()
        {
            _rewardsData = GameProgress.GetReceivedRewards();
            _diceResult.Calculate(_gameScreen.Dices);
            _playerHealthDelta = _playerData.CurrentHealth;
            _enemyHealthDelta = _enemyData.CurrentHealth;

            PlayerTurn();
            _attemptsNumber = 0;

            _gameScreen.ResetSelection();
            _gameScreen.SetContextLabel("Бросить все"); // TODO Translation

            UpdateButtonStates();
        }

        private void AnimatePlayerHealth()
        {
            _playerHealthDelta = _playerData.CurrentHealth - _playerHealthDelta;

            switch (_playerHealthDelta)
            {
                case < 0:
                    _gameScreen.PlayerAnimateDamage();
                    break;
                case > 0:
                    _gameScreen.PlayerAnimateHeal();
                    break;
            }

            _playerHealthDelta = 0;
        }

        private void AnimateEnemyHealth()
        {
            _enemyHealthDelta = _enemyData.CurrentHealth - _enemyHealthDelta;

            switch (_enemyHealthDelta)
            {
                case < 0:
                    _gameScreen.EnemyAnimateDamage();
                    break;
                case > 0:
                    _gameScreen.EnemyAnimateHeal();
                    break;
            }

            _enemyHealthDelta = 0;
        }

        #region Updates

        public void UpdateData()
        {
            _playerData.Update(_config);
            _playerData.Log();

            // UpdateData();
            UpdatePlayerStats();

            // _attemptsNumber = 0;
            // _playerHealthDelta = 0;
            // _enemyHealthDelta = 0;

            // SetMaxAttempts();
            ResetNumbers();
            UpdateDiceCount();
        }

        private void UpdateDiceCount()
        {
            // TODO Improvements in the number of cubes
            // int diceCount = GameProgress.GetDiceCount();
            // _gameScreen.SetDiceCount(_config.DiceStartCount);
        }

        private void UpdateButtonStates()
        {
            if (_attemptsNumber == 0)
            {
                _gameScreen.DisableDiceInteractable();
            }
            else if (_attemptsNumber >= _maxAttempts - 1)
            {
                _gameScreen.DisableDiceInteractable();
                _gameScreen.SetContextLabel("Закончить"); // TODO Translation
            }
            else
            {
                _gameScreen.EnableDiceInteractable();
                _gameScreen.SetContextLabel("Закончить"); // TODO Translation
            }

            ShowAttempts();
        }

        private void ShowAttempts()
        {
            int attemptsLeft = _maxAttempts - 1 - _attemptsNumber;
            // TODO Translate
            string message = $"Осталось {attemptsLeft} попыток";
            SignalSystem.Raise<IHintHandler>(handler => handler.Show(message));
        }

        private void ResetNumbers()
        {
            _attemptsNumber = 0;
            _playerHealthDelta = 0;
            _enemyHealthDelta = 0;

            SetMaxAttempts();
        }

        private void SetMaxAttempts()
        {
            RewardsData receivedRewards = GameProgress.GetReceivedRewards();
            int additionalTryCount = receivedRewards.RewardTypes.Count(reward => reward == RewardType.AdditionalTry);
            _maxAttempts = _config.MaxAttempts + additionalTryCount;
        }

        private void UpdatePlayerStats()
        {
            // int regenHealth = _rewardsData.RewardTypes.Count(r => r == RewardType.RegenHealth) * _config.Player.GrowthHealth;
            int bonusArmorCount = _rewardsData.RewardTypes.Count(r => r == RewardType.BaseArmor) * _config.Player.GrowthArmor;
            int bonusDamageCount = _rewardsData.RewardTypes.Count(r => r == RewardType.BaseDamage) * _config.Player.GrowthDamage;

            _playerData.Armor = Mathf.Max(0, bonusArmorCount);
            _playerData.Damage = Mathf.Max(0, bonusDamageCount);

            _gameScreen.UpdatePlayerStats();

            UpdateDiceCount();
            // ResetNumbers();
            SetMaxAttempts();
        }

        #endregion

        #region Player actions

        private void PlayerTurn()
        {
            ApplyPlayerHealing();
            ApplyPlayerArmor();
            ApplyPlayerAttack();

            if (_enemyData.CurrentHealth <= 0 || _config.IsInstaWin)
            {
                OnEnemyDefeated();
            }
            else
            {
                EnemyTurn();
            }

            UpdatePlayerStats();
        }

        private void ApplyPlayerHealing()
        {
            int regenHealth = _rewardsData.RewardTypes.Count(r => r == RewardType.RegenHealth) * _config.Player.GrowthHealth;
            int allRegenHealth = _diceResult.Heal + regenHealth;
            _gameScreen.PlayerTakeHeal(allRegenHealth);

            Debug.Log("Heal: Dice = " + _diceResult.Heal + ", Character = " + regenHealth);
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.PlayerHeal));
        }

        private void ApplyPlayerArmor()
        {
            int bonusArmorCount = _rewardsData.RewardTypes.Count(r => r == RewardType.BaseArmor) * _config.Player.GrowthArmor;
            _playerData.Armor = Mathf.Max(0, _diceResult.Armor + bonusArmorCount);

            Debug.Log("Armor: Dice = " + _diceResult.Armor + ", Character = " + bonusArmorCount);
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.PlayerArmor));
        }

        private void ApplyPlayerAttack()
        {
            int bonusDamageCount = _rewardsData.RewardTypes.Count(r => r == RewardType.BaseDamage) * _config.Player.GrowthDamage;
            _playerData.Damage = Mathf.Max(_diceResult.Damage, _diceResult.Damage + bonusDamageCount);
            _gameScreen.EnemyTakeDamage(_playerData.Damage);

            Debug.Log("Damage: Dice = " + _diceResult.Damage + ", Character = " + bonusDamageCount);

            // TODO You can add different sounds to attack different enemies
            // SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.SlimeAttack));
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.EnemyHit));
        }

        private void RemovePlayerArmor()
        {
            int bonusArmor = _rewardsData.RewardTypes.Count(r => r == RewardType.BaseArmor) * _config.Player.GrowthArmor;
            _playerData.Armor = Mathf.Max(0, bonusArmor);
        }

        private void RemovePlayerDamage()
        {
            int bonusDamageCount = _rewardsData.RewardTypes.Count(r => r == RewardType.BaseDamage) * _config.Player.GrowthDamage;
            _playerData.Damage = Mathf.Max(0, bonusDamageCount);
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
            AnimateEnemyHealth();

            _gameScreen.PlayerTakeDamage(_enemyData.Damage);

            // TODO You can add different sounds to attack different enemies
            // SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.SlimeAttack));
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.SlimeAttack));

            if (_playerData.CurrentHealth <= 0)
            {
                OnPlayerDefeated();
                return;
            }


            AnimatePlayerHealth();
            RemovePlayerArmor();
            RemovePlayerDamage();
        }

        private void OnEnemyDefeated()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.EnemyDefeated));
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.LootScreen));

            GameProgress.IncrementCurrentLevel();
            GameProgress.IncrementLevels();

            UpdateData();
            _enemyData = _spawner.SpawnEnemy();

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Victory));
            RemovePlayerArmor();
            UpdateButtonStates();
        }
        #endregion
    }

    public class MatchData
    {
        public int MaxAttempts;
        public int AttemptsNumber;

        public int _playerHealthDelta;
        public int _enemyHealthDelta;
    }
}

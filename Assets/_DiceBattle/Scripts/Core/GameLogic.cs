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

        private readonly MatchData _matchData = new();

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

            _matchData.EnemyData = _spawner.SpawnEnemy();
            _matchData.PlayerData = _spawner.SpawnHero();

            _gameScreen.DisableDiceInteractable();
            SignalSystem.Raise<IHintHandler>(handler => handler.Hide());
        }

        public void ContextClick()
        {
            _matchData.RemainingDiceRerolls++;

            if (_matchData.RemainingDiceRerolls == 1)
            {
                _gameScreen.RollDice();
            }
            else if (_matchData.RemainingDiceRerolls < _matchData.MaxDiceRerolls)
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
            if (_matchData.RemainingDiceRerolls <= 0)
            {
                return;
            }

            if (_matchData.RemainingDiceRerolls >= _matchData.MaxDiceRerolls)
            {
                return;
            }

            _gameScreen.ToggleAllDice();
        }

        private void EndTurn()
        {
            _matchData.RewardsData = GameProgress.GetReceivedRewards();
            _diceResult.Calculate(_gameScreen.Dices);
            _matchData.PlayerHealthChange = _matchData.PlayerData.CurrentHealth;
            _matchData.EnemyHealthChange = _matchData.EnemyData.CurrentHealth;

            PlayerTurn();
            _matchData.RemainingDiceRerolls = 0;

            _gameScreen.ResetSelection();
            _gameScreen.SetContextLabel("Бросить все"); // TODO Translation

            UpdateButtonStates();
        }

        private void AnimatePlayerHealth()
        {
            _matchData.PlayerHealthChange = _matchData.PlayerData.CurrentHealth - _matchData.PlayerHealthChange;

            switch (_matchData.PlayerHealthChange)
            {
                case < 0:
                    _gameScreen.PlayerAnimateDamage();
                    break;
                case > 0:
                    _gameScreen.PlayerAnimateHeal();
                    break;
            }

            _matchData.PlayerHealthChange = 0;
        }

        private void AnimateEnemyHealth()
        {
            _matchData.EnemyHealthChange = _matchData.EnemyData.CurrentHealth - _matchData.EnemyHealthChange;

            switch (_matchData.EnemyHealthChange)
            {
                case < 0:
                    _gameScreen.EnemyAnimateDamage();
                    break;
                case > 0:
                    _gameScreen.EnemyAnimateHeal();
                    break;
            }

            _matchData.EnemyHealthChange = 0;
        }

        #region Updates

        public void UpdateData()
        {
            _matchData.PlayerData.Update(_config);
            _matchData.PlayerData.Log();

            UpdatePlayerStats();

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
            if (_matchData.RemainingDiceRerolls == 0)
            {
                _gameScreen.DisableDiceInteractable();
            }
            else if (_matchData.RemainingDiceRerolls >= _matchData.MaxDiceRerolls - 1)
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
            int attemptsLeft = _matchData.MaxDiceRerolls - 1 - _matchData.RemainingDiceRerolls;
            // TODO Translate
            string message = $"Осталось {attemptsLeft} попыток";
            SignalSystem.Raise<IHintHandler>(handler => handler.Show(message));
        }

        private void ResetNumbers()
        {
            _matchData.RemainingDiceRerolls = 0;
            _matchData.PlayerHealthChange = 0;
            _matchData.EnemyHealthChange = 0;

            SetMaxAttempts();
        }

        private void SetMaxAttempts()
        {
            RewardsData receivedRewards = GameProgress.GetReceivedRewards();
            int additionalTryCount = receivedRewards.DiceTypes.Count(reward => reward == DiceBattle.DiceType.AdditionalTry);
            _matchData.MaxDiceRerolls = _config.MaxAttempts + additionalTryCount;
        }

        private void UpdatePlayerStats()
        {
            // int regenHealth = _rewardsData.RewardTypes.Count(r => r == RewardType.RegenHealth) * _config.Player.GrowthHealth;
            int bonusArmorCount = _matchData.RewardsData.DiceTypes.Count(r => r == DiceBattle.DiceType.BaseArmor) * _config.Player.GrowthArmor;
            int bonusDamageCount = _matchData.RewardsData.DiceTypes.Count(r => r == DiceBattle.DiceType.BaseDamage) * _config.Player.GrowthDamage;

            _matchData.PlayerData.Armor = Mathf.Max(0, bonusArmorCount);
            _matchData.PlayerData.Damage = Mathf.Max(0, bonusDamageCount);

            _gameScreen.UpdatePlayerStats();

            UpdateDiceCount();
            SetMaxAttempts();
        }

        #endregion

        #region Player actions

        private void PlayerTurn()
        {
            ApplyPlayerHealing();
            ApplyPlayerArmor();
            ApplyPlayerAttack();

            if (_matchData.EnemyData.CurrentHealth <= 0 || _config.IsInstaWin)
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
            int regenHealth = _matchData.RewardsData.DiceTypes.Count(r => r == DiceBattle.DiceType.RegenHealth) * _config.Player.GrowthHealth;
            int allRegenHealth = _diceResult.Heal + regenHealth;
            _gameScreen.PlayerTakeHeal(allRegenHealth);

            Debug.Log("Heal: Dice = " + _diceResult.Heal + ", Character = " + regenHealth);
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.PlayerHeal));
        }

        private void ApplyPlayerArmor()
        {
            int bonusArmorCount = _matchData.RewardsData.DiceTypes.Count(r => r == DiceBattle.DiceType.BaseArmor) * _config.Player.GrowthArmor;
            _matchData.PlayerData.Armor = Mathf.Max(0, _diceResult.Armor + bonusArmorCount);

            Debug.Log("Armor: Dice = " + _diceResult.Armor + ", Character = " + bonusArmorCount);
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.PlayerArmor));
        }

        private void ApplyPlayerAttack()
        {
            int bonusDamageCount = _matchData.RewardsData.DiceTypes.Count(r => r == DiceBattle.DiceType.BaseDamage) * _config.Player.GrowthDamage;
            _matchData.PlayerData.Damage = Mathf.Max(_diceResult.Damage, _diceResult.Damage + bonusDamageCount);
            _gameScreen.EnemyTakeDamage(_matchData.PlayerData.Damage);

            Debug.Log("Damage: Dice = " + _diceResult.Damage + ", Character = " + bonusDamageCount);

            // TODO You can add different sounds to attack different enemies
            // SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.SlimeAttack));
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.EnemyHit));
        }

        private void RemovePlayerArmor()
        {
            int bonusArmor = _matchData.RewardsData.DiceTypes.Count(r => r == DiceBattle.DiceType.BaseArmor) * _config.Player.GrowthArmor;
            _matchData.PlayerData.Armor = Mathf.Max(0, bonusArmor);
        }

        private void RemovePlayerDamage()
        {
            int bonusDamageCount = _matchData.RewardsData.DiceTypes.Count(r => r == DiceBattle.DiceType.BaseDamage) * _config.Player.GrowthDamage;
            _matchData.PlayerData.Damage = Mathf.Max(0, bonusDamageCount);
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

            _gameScreen.PlayerTakeDamage(_matchData.EnemyData.Damage);

            // TODO You can add different sounds to attack different enemies
            // SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.SlimeAttack));
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.SlimeAttack));

            if (_matchData.PlayerData.CurrentHealth <= 0)
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
            _matchData.EnemyData = _spawner.SpawnEnemy();

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Victory));
            RemovePlayerArmor();
            UpdateButtonStates();
        }
        #endregion
    }
}

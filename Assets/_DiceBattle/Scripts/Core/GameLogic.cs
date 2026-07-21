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
        private bool _battleEnded;

        private UnitConfig PlayerConfig => _config.GetPlayerConfig(GameData.SelectedCharacterClass);

        public GameLogic(GameConfig config, GameScreen gameScreen)
        {
            _config = config;
            _gameScreen = gameScreen;
            _spawner = new Spawner(config, gameScreen);
        }

        public void InitializeGame()
        {
            _battleEnded = false;
            ResetNumbers();
            UpdateDiceCount();
            _gameScreen.ResetDice();

            _matchData.EnemyData = _spawner.SpawnEnemy();
            _matchData.PlayerData = _spawner.SpawnHero();
            _matchData.LastStandUsed = false;

            _gameScreen.DisableDiceInteractable();
            _gameScreen.ClearPlayerDicePreview();
            SignalSystem.Raise<IHintHandler>(handler => handler.Hide());

            if (_config.CanSaveBattle)
                BattleSaveData.Save(_matchData);
        }

        public void AbandonBattle()
        {
            _battleEnded = true;

            if (_config.CanSaveBattle)
                BattleSaveData.Clear();
        }

        public void RestoreGame()
        {
            _battleEnded = false;
            ResetNumbers();
            UpdateDiceCount();
            _gameScreen.ResetDice();

            BattleSnapshot saved = BattleSaveData.Load();

            _matchData.EnemyData = _spawner.RestoreEnemy(saved);
            _matchData.PlayerData = _spawner.RestoreHero(saved);
            _matchData.MaxDiceRerolls = saved.MaxDiceRerolls;
            _matchData.RemainingDiceRerolls = saved.RemainingDiceRerolls;
            _matchData.LastStandUsed = saved.LastStandUsed;

            _gameScreen.DisableDiceInteractable();
            _gameScreen.ClearPlayerDicePreview();
            SignalSystem.Raise<IHintHandler>(handler => handler.Hide());
        }

        public void UpdateDicePreview()
        {
            DiceList equippedItems = GameData.GetEquippedAsDiceList();
            _diceResult.Calculate(_gameScreen.Dices, equippedItems);

            int regenHealth = equippedItems.DiceTypes.Count(r => r == DiceType.RegenHealth) * PlayerConfig.GrowthHealth;
            int bonusArmorCount = equippedItems.DiceTypes.Count(r => r == DiceType.BaseArmor) * PlayerConfig.GrowthArmor;
            int bonusDamageCount = equippedItems.DiceTypes.Count(r => r == DiceType.BaseDamage) * PlayerConfig.GrowthDamage;

            _gameScreen.SetPlayerDicePreview(
                Mathf.Max(0, _diceResult.Armor + bonusArmorCount),
                Mathf.Max(0, _diceResult.Damage + bonusDamageCount),
                _diceResult.Heal + regenHealth);
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

            if (_gameScreen.HaveUnselectedDice)
            {
                _gameScreen.SetSelectionStatus(true);
            }
            else
            {
                _gameScreen.SetSelectionStatus(false);
            }

            // _gameScreen.ToggleAllDice();
        }

        private void EndTurn()
        {
            _matchData.DiceList = GameData.GetEquippedAsDiceList();
            _diceResult.Calculate(_gameScreen.Dices, _matchData.DiceList);
            _matchData.PlayerHealthChange = _matchData.PlayerData.CurrentHealth;
            _matchData.EnemyHealthChange = _matchData.EnemyData.CurrentHealth;

            _gameScreen.ClearPlayerDicePreview();
            PlayerTurn();
            _matchData.RemainingDiceRerolls = 0;

            _gameScreen.ResetSelection();
            _gameScreen.SetContextLabel("Бросить все"); // TODO Translation

            SignalSystem.Raise<IHintHandler>(handler => handler.Hide());
            UpdateButtonStates();

            if (_config.CanSaveBattle && _battleEnded == false)
                BattleSaveData.Save(_matchData);
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
            DiceList equippedItems = GameData.GetEquippedAsDiceList();
            int additionalDiceCount = equippedItems.DiceTypes.Count(r => r == DiceType.AdditionalDice);
            _gameScreen.SetDiceCount(_config.DiceStartCount + additionalDiceCount);
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
            DiceList receivedRewards = GameData.GetEquippedAsDiceList();
            int additionalTryCount = receivedRewards.DiceTypes.Count(reward => reward == DiceType.AdditionalTry);
            _matchData.MaxDiceRerolls = _config.MaxAttempts + additionalTryCount;
        }

        private void UpdatePlayerStats()
        {
            // int regenHealth = _rewardsData.RewardTypes.Count(r => r == RewardType.RegenHealth) * PlayerConfig.GrowthHealth;
            int bonusArmorCount = _matchData.DiceList.DiceTypes.Count(r => r == DiceType.BaseArmor) * PlayerConfig.GrowthArmor;
            int bonusDamageCount = _matchData.DiceList.DiceTypes.Count(r => r == DiceType.BaseDamage) * PlayerConfig.GrowthDamage;

            _matchData.PlayerData.Armor = Mathf.Max(0, PlayerConfig.StartArmor + bonusArmorCount);
            _matchData.PlayerData.Damage = Mathf.Max(0, PlayerConfig.StartDamage + bonusDamageCount);

            _gameScreen.SetPlayerEquipmentBonus(bonusArmorCount, bonusDamageCount);
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

            AnimateEnemyHealth();

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
            int regenHealth = _matchData.DiceList.DiceTypes.Count(r => r == DiceType.RegenHealth) * PlayerConfig.GrowthHealth;
            int allRegenHealth = _diceResult.Heal + regenHealth;
            _gameScreen.PlayerTakeHeal(allRegenHealth);

            Debug.Log("Heal: Dice = " + _diceResult.Heal + ", Character = " + regenHealth);
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.PlayerHeal));
        }

        private void ApplyPlayerArmor()
        {
            int bonusArmorCount = _matchData.DiceList.DiceTypes.Count(r => r == DiceType.BaseArmor) * PlayerConfig.GrowthArmor;
            _matchData.PlayerData.Armor = Mathf.Max(0, PlayerConfig.StartArmor + _diceResult.Armor + bonusArmorCount);
            _gameScreen.SetPlayerEquipmentBonus(bonusArmorCount, null);

            Debug.Log("Armor: Dice = " + _diceResult.Armor + ", Character = " + bonusArmorCount);
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.PlayerArmor));
        }

        private void ApplyPlayerAttack()
        {
            int bonusDamageCount = _matchData.DiceList.DiceTypes.Count(r => r == DiceType.BaseDamage) * PlayerConfig.GrowthDamage;
            _matchData.PlayerData.Damage = Mathf.Max(0, PlayerConfig.StartDamage + _diceResult.Damage + bonusDamageCount);
            _gameScreen.SetPlayerEquipmentBonus(null, bonusDamageCount);
            _gameScreen.EnemyTakeDamage(_matchData.PlayerData.Damage);

            Debug.Log("Damage: Dice = " + _diceResult.Damage + ", Character = " + bonusDamageCount);

            ApplyLifesteal();

            // TODO You can add different sounds to attack different enemies
            // SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.SlimeAttack));
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.EnemyHit));
        }

        private void ApplyLifesteal()
        {
            int lifestealCount = _matchData.DiceList.DiceTypes.Count(r => r == DiceType.LifestealDice);

            if (lifestealCount == 0)
            {
                return;
            }

            int lifestealHeal = _matchData.PlayerData.Damage / 4 * lifestealCount;
            _gameScreen.PlayerTakeHeal(lifestealHeal);
        }

        private void RemovePlayerArmor()
        {
            int bonusArmor = _matchData.DiceList.DiceTypes.Count(r => r == DiceBattle.DiceType.BaseArmor) * PlayerConfig.GrowthArmor;
            _matchData.PlayerData.Armor = Mathf.Max(0, PlayerConfig.StartArmor + bonusArmor);
            _gameScreen.SetPlayerEquipmentBonus(bonusArmor, null);
        }

        private void RemovePlayerDamage()
        {
            int bonusDamageCount = _matchData.DiceList.DiceTypes.Count(r => r == DiceBattle.DiceType.BaseDamage) * PlayerConfig.GrowthDamage;
            _matchData.PlayerData.Damage = Mathf.Max(0, PlayerConfig.StartDamage + bonusDamageCount);
            _gameScreen.SetPlayerEquipmentBonus(null, bonusDamageCount);
        }

        private void OnPlayerDefeated()
        {
            _battleEnded = true;

            if (_config.CanSaveBattle)
                BattleSaveData.Clear();

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Defeat));
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.GameOverScreen));
        }

        #endregion

        #region Enemy actions

        private void EnemyTurn()
        {
            _gameScreen.PlayerTakeDamage(_matchData.EnemyData.Damage);

            // TODO You can add different sounds to attack different enemies
            // SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.SlimeAttack));
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.SlimeAttack));

            if (_matchData.PlayerData.CurrentHealth <= 0 && TryTriggerLastStand() == false)
            {
                OnPlayerDefeated();
                return;
            }


            AnimatePlayerHealth();
            RemovePlayerArmor();
            RemovePlayerDamage();
        }

        private bool TryTriggerLastStand()
        {
            if (_matchData.LastStandUsed)
            {
                return false;
            }

            bool hasLastStandDice = _matchData.DiceList.DiceTypes.Contains(DiceType.LastStandDice);

            if (hasLastStandDice == false)
            {
                return false;
            }

            _matchData.LastStandUsed = true;
            _gameScreen.PlayerTakeHeal(1);

            return true;
        }

        private void OnEnemyDefeated()
        {
            _battleEnded = true;

            if (_config.CanSaveBattle)
                BattleSaveData.Clear();

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.EnemyDefeated));

            bool isLastEnemy = GameData.CompletedLevels >= _config.Enemies.Count - 1;
            _matchData.IsLastEnemy = isLastEnemy;

            if (isLastEnemy)
            {
                SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.GameOverScreen));
            }
            else
            {
                SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.LootScreen));
            }

            GameData.IncrementLevels();

            UpdateData();
            // Спавн следующего врага отключён: после победы враг остаётся повержен (0 здоровья) до возврата в таверну.
            // _matchData.EnemyData = _spawner.SpawnEnemy();

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Victory));
            RemovePlayerArmor();
            UpdateButtonStates();
        }
        #endregion
    }
}

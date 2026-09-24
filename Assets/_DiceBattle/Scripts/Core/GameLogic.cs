using System.Linq;
using Assets.SimpleLocalization.Scripts;
using DiceBattle.Audio;
using DiceBattle.Data;
using DiceBattle.Events;
using DiceBattle.Global;
using DiceBattle.Localization;
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

        private const float _battleEndPause = 1f;

        private readonly MatchData _matchData = new();
        private bool _battleEnded;

        public bool IsBattleEnded => _battleEnded;
        private bool _isRolling;
        private int _battleEndTweenId = -1;

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
            _isRolling = false;
            ResetNumbers();
            UpdateDeck();
            _gameScreen.ResetDice();

            _matchData.EnemyData = _spawner.SpawnEnemy();
            _matchData.PlayerData = _spawner.SpawnHero();
            _matchData.LastStandUsed = false;

            _gameScreen.DisableDiceInteractable();
            _gameScreen.ClearPlayerDicePreview();
            SignalSystem.Raise<IHintHandler>(handler => handler.Hide());

            if (_config.CanSaveBattle)
            {
                BattleSaveData.Save(_matchData);
            }
        }

        public void AbandonBattle()
        {
            _battleEnded = true;
            LeanTween.cancel(_battleEndTweenId);

            if (_config.CanSaveBattle)
            {
                BattleSaveData.Clear();
            }
        }

        public void RestoreGame()
        {
            _battleEnded = false;
            _isRolling = false;
            ResetNumbers();
            UpdateDeck();
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

        public void OnRollCompleted()
        {
            if (_isRolling == false)
            {
                return;
            }

            _isRolling = false;
            UpdateDicePreview();
            UpdateButtonStates();
        }

        // Equipment bonuses are already shown by UnitPanel, so the preview carries dice results only.
        private void UpdateDicePreview()
        {
            _diceResult.Calculate(_gameScreen.Dices);
            _gameScreen.SetPlayerDicePreview(_diceResult.Armor, _diceResult.Damage, _diceResult.Heal);
        }

        public void ContextClick()
        {
            if (_battleEnded || _isRolling)
            {
                return;
            }

            _matchData.RemainingDiceRerolls++;

            if (_matchData.RemainingDiceRerolls == 1)
            {
                GameData.HasEverRolledDice = true;
                _isRolling = true;
                _gameScreen.RollDice();
            }
            else if (_matchData.RemainingDiceRerolls < _matchData.MaxDiceRerolls)
            {
                if (_gameScreen.HaveSelectedDice)
                {
                    _isRolling = true;
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
            if (_isRolling || _matchData.RemainingDiceRerolls <= 0)
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
            _diceResult.Calculate(_gameScreen.Dices);
            _matchData.PlayerHealthChange = _matchData.PlayerData.CurrentHealth;
            _matchData.EnemyHealthChange = _matchData.EnemyData.CurrentHealth;

            _gameScreen.ClearPlayerDicePreview();
            PlayerTurn();
            _matchData.RemainingDiceRerolls = 0;

            _gameScreen.ResetSelection();
            _gameScreen.SetContextLabel(LocalizationManager.Localize(LocKeys.Button.RollAll));

            SignalSystem.Raise<IHintHandler>(handler => handler.Hide());
            UpdateButtonStates();

            if (_config.CanSaveBattle && _battleEnded == false)
            {
                BattleSaveData.Save(_matchData);
            }
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
            UpdateDeck();
        }

        private void UpdateDeck()
        {
            _gameScreen.SetDeck(DiceRuleset.Deck(_config.DiceStartCount));
        }

        private void UpdateButtonStates()
        {
            if (_matchData.RemainingDiceRerolls == 0 || _isRolling)
            {
                _gameScreen.DisableDiceInteractable();
            }
            else if (_matchData.RemainingDiceRerolls >= _matchData.MaxDiceRerolls - 1)
            {
                _gameScreen.DisableDiceInteractable();
                _gameScreen.SetContextLabel(LocalizationManager.Localize(LocKeys.GameHits.Finish));
            }
            else
            {
                _gameScreen.EnableDiceInteractable();
                _gameScreen.SetContextLabel(LocalizationManager.Localize(LocKeys.GameHits.Finish));
            }

            ShowAttempts();
        }

        private void ShowAttempts()
        {
            int attemptsLeft = _matchData.MaxDiceRerolls - 1 - _matchData.RemainingDiceRerolls;

            string message = LocalizationManager.Localize(LocKeys.Message.AttemptsLeft, attemptsLeft);
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
            _matchData.PlayerData.Armor = Mathf.Max(0, PlayerConfig.StartArmor);
            _matchData.PlayerData.Damage = Mathf.Max(0, PlayerConfig.StartDamage);

            _gameScreen.UpdatePlayerStats();

            UpdateDeck();
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
            _gameScreen.PlayerTakeHeal(_diceResult.Heal);

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.PlayerHeal));
        }

        private void ApplyPlayerArmor()
        {
            _matchData.PlayerData.Armor = Mathf.Max(0, PlayerConfig.StartArmor + _diceResult.Armor);

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.PlayerArmor));
        }

        private void ApplyPlayerAttack()
        {
            _matchData.PlayerData.Damage = Mathf.Max(0, PlayerConfig.StartDamage + _diceResult.Damage);

            if (_diceResult.IsCritical)
            {
                _gameScreen.EnemyTakeCriticalHit();
            }
            else
            {
                _gameScreen.EnemyTakeDamage(_matchData.PlayerData.Damage);
            }

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.EnemyHit));
        }

        private void RemovePlayerArmor()
        {
            _matchData.PlayerData.Armor = Mathf.Max(0, PlayerConfig.StartArmor);
        }

        private void RemovePlayerDamage()
        {
            _matchData.PlayerData.Damage = Mathf.Max(0, PlayerConfig.StartDamage);
        }

        private void OnPlayerDefeated()
        {
            _battleEnded = true;

            if (_config.CanSaveBattle)
                BattleSaveData.Clear();

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Defeat));
            AfterBattleEndPause(() => SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.GameOverScreen)));
        }

        #endregion

        #region Enemy actions

        private void EnemyTurn()
        {
            _gameScreen.PlayerTakeDamage(_matchData.EnemyData.Damage);

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

            bool hasLastStandDice = _matchData.DiceList.DiceTypes.Contains(DiceType.LastStand);

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
                AfterBattleEndPause(() => SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.GameOverScreen)));
            }
            else
            {
                GameData.SetPendingLootReward(GameData.CompletedLevels);
                AfterBattleEndPause(() => SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.LootScreen)));
            }

            GameData.IncrementLevels();

            UpdateData();
            // Спавн следующего врага отключён: после победы враг остаётся повержен (0 здоровья) до возврата в таверну.
            // _matchData.EnemyData = _spawner.SpawnEnemy();

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Victory));
            RemovePlayerArmor();
            UpdateButtonStates();
        }

        // Lets the final hit and floating numbers play out before the result screen covers them.
        private void AfterBattleEndPause(System.Action showResult)
        {
            _battleEndTweenId = LeanTween.delayedCall(_battleEndPause, showResult).id;
        }
        #endregion
    }
}

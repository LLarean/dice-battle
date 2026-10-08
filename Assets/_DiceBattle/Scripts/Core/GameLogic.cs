using System.Linq;
using Assets.SimpleLocalization.Scripts;
using DiceBattle.Audio;
using DiceBattle.Auxiliary;
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
        private const float _playerHitDelay = 0.2f;
        private const float _enemyAttackDelay = 0.4f;
        private const float _enemyHitDelay = 0.65f;

        private readonly MatchData _matchData = new();
        private bool _battleEnded;

        public bool IsBattleEnded => _battleEnded;
        private bool _isRolling;
        private int _battleEndTweenId = -1;
        private bool _isTurnResolving;
        private readonly int[] _turnTweenIds = { -1, -1, -1 };

        private UnitConfig PlayerConfig => _config.GetPlayerConfig(GameData.SelectedCharacterClass);

        public GameLogic(GameConfig config, GameScreen gameScreen)
        {
            _config = config;
            _gameScreen = gameScreen;
            _spawner = new Spawner(config, gameScreen);
        }

        public void InitializeGame()
        {
            CancelTurnTweens();
            _battleEnded = false;
            _isRolling = false;
            ResetNumbers();
            UpdateDeck();
            _gameScreen.ResetDice();

            _matchData.EnemyData = _spawner.SpawnEnemy();
            _matchData.PlayerData = _spawner.SpawnHero();
            _matchData.LastStandUsed = false;

            _gameScreen.DisableDiceInteractable();
            _gameScreen.SetContextLabel(LocalizationManager.Localize(LocKeys.Button.RollAll));
            _gameScreen.SetContextAvailable(true);
            _gameScreen.ClearPlayerDicePreview();
            SignalSystem.Raise<IHintHandler>(handler => handler.Hide());

            SaveBattle();
        }

        public void AbandonBattle()
        {
            _battleEnded = true;
            LeanTween.cancel(_battleEndTweenId);
            CancelTurnTweens();

            if (_config.CanSaveBattle)
            {
                BattleSaveData.Clear();
            }
        }

#if UNITY_EDITOR
        public void DebugSetHealth(int playerHealth, int enemyHealth)
        {
            _matchData.PlayerData.CurrentHealth = Mathf.Clamp(playerHealth, 1, _matchData.PlayerData.MaxHealth);
            _matchData.EnemyData.CurrentHealth = Mathf.Clamp(enemyHealth, 1, _matchData.EnemyData.MaxHealth);
        }
#endif

        public void RestoreGame()
        {
            CancelTurnTweens();
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
            _gameScreen.SetContextLabel(LocalizationManager.Localize(LocKeys.Button.RollAll));
            _gameScreen.SetContextAvailable(true);
            _gameScreen.ClearPlayerDicePreview();
            SignalSystem.Raise<IHintHandler>(handler => handler.Hide());

            if (saved.RemainingDiceRerolls > 0)
            {
                _gameScreen.SetDiceFaces(saved.DiceFaces);
                UpdateDicePreview();
                UpdateButtonStates();
            }
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
            SaveBattle();
        }

        // Equipment bonuses are already shown by UnitPanel, so the preview carries dice results only.
        private void UpdateDicePreview()
        {
            _diceResult.Calculate(_gameScreen.Dices);
            _gameScreen.SetPlayerDicePreview(_diceResult.Armor, _diceResult.Damage, _diceResult.Heal);
        }

        public void ContextClick()
        {
            if (_battleEnded || _isRolling || _isTurnResolving)
            {
                return;
            }

            _matchData.RemainingDiceRerolls++;

            if (_matchData.RemainingDiceRerolls == 1)
            {
                GameData.HasEverRolledDice = true;
                StartRoll();
                _gameScreen.RollDice();
            }
            else if (_matchData.RemainingDiceRerolls < _matchData.MaxDiceRerolls)
            {
                if (_gameScreen.HaveSelectedDice)
                {
                    StartRoll();
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

        // The attempt is saved as spent before the faces change, so quitting mid-roll cannot win it back.
        private void StartRoll()
        {
            _isRolling = true;
            SaveBattle();
        }

        private void SaveBattle()
        {
            if (_config.CanSaveBattle && _battleEnded == false && _isTurnResolving == false)
            {
                BattleSaveData.Save(_matchData, _gameScreen.Dices);
            }
        }

        private void EndTurn()
        {
            _matchData.DiceList = GameData.GetEquippedAsDiceList();
            _diceResult.Calculate(_gameScreen.Dices);
            _gameScreen.ClearPlayerDicePreview();
            PlayerTurn();
            _matchData.RemainingDiceRerolls = 0;

            _gameScreen.ResetSelection();
            _gameScreen.SetContextLabel(LocalizationManager.Localize(LocKeys.Button.RollAll));

            SignalSystem.Raise<IHintHandler>(handler => handler.Hide());
            UpdateButtonStates();
            SaveBattle();
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

            _gameScreen.SetContextAvailable(_isRolling == false && _battleEnded == false && _isTurnResolving == false);
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

            SetMaxAttempts();
        }

        private void SetMaxAttempts()
        {
            DiceList receivedRewards = GameData.GetEquippedAsDiceList();
            int additionalTryCount = receivedRewards.DiceTypes.Count(reward => reward == DiceType.AdditionalTry);
            _matchData.MaxDiceRerolls = DebugOverrides.HasInfiniteRerolls
                ? DebugOverrides.InfiniteRerolls
                : _config.MaxAttempts + additionalTryCount;
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

        // The swing, the hit and the enemy's answer are spread in time so that each has its own sound and reaction.
        // Nothing is saved until the enemy has answered: quitting in between replays the turn with the same dice.
        private void PlayerTurn()
        {
            _isTurnResolving = true;

            ApplyPlayerHealing();
            ApplyPlayerArmor();

            PlaySound(PlayerConfig.AttackSound);
            _turnTweenIds[0] = LeanTween.delayedCall(_playerHitDelay, CompletePlayerAttack).id;
        }

        private void CompletePlayerAttack()
        {
            ApplyPlayerAttack();

            if (_matchData.EnemyData.CurrentHealth <= 0 || DebugOverrides.IsInstaWin)
            {
                _isTurnResolving = false;
                OnEnemyDefeated();
                UpdatePlayerStats();
                return;
            }

            SoundType attackSound = _matchData.EnemyData.AttackSound;
            _turnTweenIds[1] = LeanTween.delayedCall(_enemyAttackDelay, () => PlaySound(attackSound)).id;
            _turnTweenIds[2] = LeanTween.delayedCall(_enemyHitDelay, CompleteEnemyTurn).id;
        }

        private void CompleteEnemyTurn()
        {
            _isTurnResolving = false;

            EnemyTurn();
            UpdatePlayerStats();
            UpdateButtonStates();
            SaveBattle();
        }

        private void CancelTurnTweens()
        {
            _isTurnResolving = false;

            for (int i = 0; i < _turnTweenIds.Length; i++)
            {
                LeanTween.cancel(_turnTweenIds[i]);
                _turnTweenIds[i] = -1;
            }
        }

        private static void PlaySound(SoundType soundType)
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(soundType));
        }

        private void ApplyPlayerHealing()
        {
            int healthBefore = _matchData.PlayerData.CurrentHealth;
            _gameScreen.PlayerTakeHeal(_diceResult.Heal);

            if (_matchData.PlayerData.CurrentHealth > healthBefore)
            {
                _gameScreen.PlayerAnimateHeal();
            }

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

            _gameScreen.EnemyAnimateDamage();
            PlaySound(PlayerConfig.ImpactSound);
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

            GameData.PendingInnkeeperEvent = InnkeeperEvent.Defeat;

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Defeat));
            AfterBattleEndPause(() => SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.GameOverScreen)));
        }

        #endregion

        #region Enemy actions

        private void EnemyTurn()
        {
            int healthBefore = _matchData.PlayerData.CurrentHealth;
            _gameScreen.PlayerTakeDamage(_matchData.EnemyData.Damage);

            PlaySound(_matchData.EnemyData.ImpactSound);

            if (_matchData.PlayerData.CurrentHealth < healthBefore)
            {
                _gameScreen.PlayerAnimateDamage();
            }
            else
            {
                _gameScreen.PlayerAnimateBlock();
            }

            bool isLethal = _matchData.PlayerData.CurrentHealth <= 0 && TryTriggerLastStand() == false;

            if (isLethal || DebugOverrides.IsInstaLose)
            {
                OnPlayerDefeated();
                return;
            }

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

            GameData.IncrementTotalVictories();

            if (isLastEnemy)
            {
                GameData.PendingInnkeeperEvent = InnkeeperEvent.CampaignWon;
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

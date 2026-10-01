#if UNITY_EDITOR
using System;
using DiceBattle.Core;
using DiceBattle.Data;
using DiceBattle.Events;
using DiceBattle.Global;
using DiceBattle.UI;
using GameSignals;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace DiceBattle.Auxiliary
{
    public class DebugOptions : MonoBehaviour
    {
        [SerializeField] private DefaultInventory _defaultInventory;

        [Header("On start (applied on every Play)")]
        [Tooltip("Wipes campaign progress, inventory, battle save, tournament and volume on every Play. Turn off to test save/restore.")]
        [SerializeField] private bool _needResetAll;
        [Tooltip("Adds one dice of every type to the unequipped inventory on every Play.")]
        [SerializeField] private bool _needAddAllItemsToInventory;

        [Header("Battle cheats (campaign only, applied live)")]
        [Tooltip("Enemy dies after the player's next attack, whatever the damage.")]
        [SerializeField] private bool _isInstaWin;
        [Tooltip("Player dies after the enemy's next attack, even with Last Stand.")]
        [SerializeField] private bool _isInstaLose;
        [Tooltip("999 rerolls per turn. Takes effect from the next turn and is written into the battle save.")]
        [SerializeField] private bool _hasInfiniteRerolls;
        [Tooltip("Face per dice slot, used instead of a random roll. Empty list disables it. 5+ Attack faces = crit.")]
        [SerializeField] private DiceValue[] _forcedFaces;

        [Header("Battle: Set Health button")]
        [Tooltip("Clamped to 1..max health.")]
        [SerializeField] private int _playerHealth = 1;
        [Tooltip("Clamped to 1..max health.")]
        [SerializeField] private int _enemyHealth = 1;

        [Header("Campaign buttons")]
        [Tooltip("Completed levels, 0 = first enemy. Enemies count = full clear (New Game+ offer in the tavern).")]
        [SerializeField] private int _level;
        [Tooltip("0 = first run. Enemy stats use GameConfig.NewGamePlusMultipliers[cycle - 1].")]
        [SerializeField] private int _newGamePlusCycle;

        [Header("Loot: Show Loot button")]
        [Tooltip("Level whose 3 rewards are shown: pool offset = level * 3.")]
        [SerializeField] private int _lootLevel;

        [Header("Inventory: Add Dice buttons")]
        [SerializeField] private DiceType _diceType;

        [Header("Tournament: Set Opponents button")]
        [Tooltip("Opponent order from first fight to final. Restarts the tournament with this bracket.")]
        [SerializeField] private CharacterClass[] _opponents;

        private static GameConfig Config =>
            AssetDatabase.LoadAssetAtPath<GameConfig>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:GameConfig")[0]));

        private void Awake() => ApplyCheats();

        private void OnValidate() => ApplyCheats();

        private void Start()
        {
            if (_needResetAll)
            {
                ResetAll();
            }

            if (_needAddAllItemsToInventory)
            {
                AddAllItems();
            }
        }

        private void ApplyCheats()
        {
            DebugOverrides.IsInstaWin = _isInstaWin;
            DebugOverrides.IsInstaLose = _isInstaLose;
            DebugOverrides.HasInfiniteRerolls = _hasInfiniteRerolls;
            DebugOverrides.ForcedFaces = _forcedFaces;
        }

        #region Battle

        [Button("Battle: Set Health", enabledMode: EButtonEnableMode.Playmode)]
        private void SetHealth()
        {
            GameScreen gameScreen = FindFirstObjectByType<GameScreen>();

            if (gameScreen == null || gameScreen.IsBattleEnded)
            {
                Debug.LogWarning("No campaign battle in progress");
                return;
            }

            gameScreen.DebugSetHealth(_playerHealth, _enemyHealth);
        }

        #endregion

        #region Campaign

        [Button("Campaign: Reset All (progress, inventory, tournament, volume)")]
        private void ResetAll()
        {
            GameData.ResetAll();
            TournamentBracket.Clear();
            GameSettings.ResetVolume();
            _defaultInventory.SetDefaultInventory();
        }

        [Button("Campaign: Set Level (clears battle save)")]
        private void SetLevel() => SetCompletedLevels(Mathf.Clamp(_level, 0, Config.Enemies.Count));

        [Button("Campaign: Jump To Last Enemy (clears battle save)")]
        private void JumpToLastEnemy() => SetCompletedLevels(Config.Enemies.Count - 1);

        [Button("Campaign: Set New Game+ Cycle (clears battle save)")]
        private void SetNewGamePlusCycle()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.NewGamePlusCycle, Mathf.Max(0, _newGamePlusCycle));
            BattleSaveData.Clear();
        }

        private static void SetCompletedLevels(int level)
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.CompletedLevels, level);
            PlayerPrefs.SetInt(PlayerPrefsKeys.CurrentLevel, level);
            GameData.ClearPendingLootReward();
            BattleSaveData.Clear();
        }

        #endregion

        #region Loot

        [Button("Loot: Show Loot For Level", enabledMode: EButtonEnableMode.Playmode)]
        private void ShowLoot()
        {
            GameData.SetPendingLootReward(Mathf.Max(0, _lootLevel));
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.LootScreen));
        }

        [Button("Loot: Regenerate Rewards Pool")]
        private void RegenerateRewardsPool()
        {
            AvailableRewardsPool.Clear();
            GameData.SaveRandomRewards(GameData.LoadRandomRewards());
            GameData.LogRandomRewards();
        }

        [Button("Loot: Log Rewards Pool")]
        private void LogRewardsPool() => GameData.LogRandomRewards();

        #endregion

        #region Inventory

        [Button("Inventory: Add One Of Every Dice")]
        private void AddAllItems()
        {
            foreach (DiceType diceType in Enum.GetValues(typeof(DiceType)))
            {
                Inventory.AddItemToUnequipped(new Item { Type = diceType, IsEquipped = false });
            }
        }

        [Button("Inventory: Add Dice (unequipped)")]
        private void AddDiceToInventory() => Inventory.AddItemToUnequipped(new Item { Type = _diceType, IsEquipped = false });

        [Button("Inventory: Add Dice (equipped, ignores slot limit)")]
        private void AddEquippedDice() => Inventory.AddItemToUnequipped(new Item { Type = _diceType, IsEquipped = true });

        [Button("Inventory: Equip All (ignores slot limit)")]
        private void EquipAllItems()
        {
            foreach (Item item in Inventory.UnequippedItems())
            {
                Inventory.EquipItem(item);
            }
        }

        #endregion

        #region Tournament

        [Button("Tournament: Reset")]
        private void ResetTournament() => TournamentBracket.Clear();

        [Button("Tournament: Set Opponents")]
        private void SetTournamentOpponents() => TournamentBracket.DebugSetOpponents(_opponents);

        [Button("Tournament: Skip To Final")]
        private void SkipToTournamentFinal() => TournamentBracket.DebugSkipToFinal();

        [Button("Tournament: Win Current Match", enabledMode: EButtonEnableMode.Playmode)]
        private void WinTournamentMatch() => EndTournamentMatch(playerWon: true);

        [Button("Tournament: Lose Current Match", enabledMode: EButtonEnableMode.Playmode)]
        private void LoseTournamentMatch() => EndTournamentMatch(playerWon: false);

        private static void EndTournamentMatch(bool playerWon)
        {
            TournamentScreen tournamentScreen = FindFirstObjectByType<TournamentScreen>();

            if (tournamentScreen == null)
            {
                Debug.LogWarning("No tournament match in progress");
                return;
            }

            tournamentScreen.DebugEndMatch(playerWon);
        }

        #endregion

        #region Saves

        [Button("Saves: Log PlayerPrefs State")]
        private void LogSaves()
        {
            Debug.Log($"<color=yellow>Saves:</color> level {GameData.CompletedLevels}, NG+ {GameData.NewGamePlusCycle}, " +
                      $"class {GameData.SelectedCharacterClass}, pending loot {PrefOrNone(PlayerPrefsKeys.PendingLootRewardIndex)}\n" +
                      $"Battle: {PrefOrNone(PlayerPrefsKeys.BattleState)}\n" +
                      $"Tournament: {PrefOrNone(PlayerPrefsKeys.TournamentState)}\n" +
                      $"Inventory: {PrefOrNone(PlayerPrefsKeys.AllItems)}");
        }

        [Button("Saves: Clear Battle Save")]
        private void ClearBattleSave() => BattleSaveData.Clear();

        [Button("Saves: Corrupt Battle Save (soft-lock test)")]
        private void CorruptBattleSave()
        {
            PlayerPrefs.SetString(PlayerPrefsKeys.BattleState, "{corrupted");
            PlayerPrefs.Save();
        }

        private static string PrefOrNone(string key)
        {
            if (PlayerPrefs.HasKey(key) == false)
            {
                return "none";
            }

            string value = PlayerPrefs.GetString(key, null);
            return string.IsNullOrEmpty(value) ? PlayerPrefs.GetInt(key).ToString() : value;
        }

        #endregion
    }
}
#endif

using System.Collections.Generic;
using System.Linq;
using DiceBattle.Data;
using DiceBattle.UI;
using UnityEngine;

namespace DiceBattle.Global
{
    public static class GameData
    {
        public static CharacterClass SelectedCharacterClass
        {
            get => (CharacterClass)PlayerPrefs.GetInt(PlayerPrefsKeys.SelectedCharacterClass, 0);
            set => PlayerPrefs.SetInt(PlayerPrefsKeys.SelectedCharacterClass, (int)value);
        }

        public static DiceList GetEquippedAsDiceList()
        {
            var list = new DiceList();
            foreach (Item item in Inventory.EquippedItems())
                list.DiceTypes.Add(item.Type);
            return list;
        }


        public static bool HasEverRolledDice
        {
            get => PlayerPrefs.GetInt(PlayerPrefsKeys.HasEverRolledDice, 0) == 1;
            set => PlayerPrefs.SetInt(PlayerPrefsKeys.HasEverRolledDice, value ? 1 : 0);
        }

        public static int CompletedLevels => PlayerPrefs.GetInt(PlayerPrefsKeys.CompletedLevels, 0);
        public static int CurrentLevel => PlayerPrefs.GetInt(PlayerPrefsKeys.CurrentLevel, 0);
        public static int NewGamePlusCycle => PlayerPrefs.GetInt(PlayerPrefsKeys.NewGamePlusCycle, 0);

        public static void ResetAll()
        {
            ResetCompletedLevels();
            ResetCurrentLevel();
            ResetNewGamePlusCycle();

            Inventory.Clear();
            ClearRandomRewards();
            ResetSelectedCharacterClass();
            BattleSaveData.Clear();
            ClearPendingLootReward();
        }

        public static void AdvanceNewGamePlus()
        {
            int nextCycle = NewGamePlusCycle + 1;
            ResetAll();
            PlayerPrefs.SetInt(PlayerPrefsKeys.NewGamePlusCycle, nextCycle);
        }

        public static void ResetNewGamePlusCycle() => PlayerPrefs.DeleteKey(PlayerPrefsKeys.NewGamePlusCycle);

        public static void ResetSelectedCharacterClass() => PlayerPrefs.DeleteKey(PlayerPrefsKeys.SelectedCharacterClass);

        public static void IncrementLevels()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.CompletedLevels, CompletedLevels + 1);
            PlayerPrefs.SetInt(PlayerPrefsKeys.CurrentLevel, CurrentLevel + 1);
        }

        public static void IncrementCompletedLevels() => PlayerPrefs.SetInt(PlayerPrefsKeys.CompletedLevels, CompletedLevels + 1);

        public static void ResetCompletedLevels() => PlayerPrefs.DeleteKey(PlayerPrefsKeys.CompletedLevels);

        public static void IncrementCurrentLevel() => PlayerPrefs.SetInt(PlayerPrefsKeys.CurrentLevel, CurrentLevel + 1);

        public static void ResetCurrentLevel() => PlayerPrefs.DeleteKey(PlayerPrefsKeys.CurrentLevel);

        #region Random Rewards

        public static DiceList LoadRandomRewards() => AvailableRewardsPool.Load();

        public static void SaveRandomRewards(DiceList diceList) => AvailableRewardsPool.Save(diceList);

        public static List<DiceType> GetRandomRewards(int startIndex, int count) =>
            AvailableRewardsPool.GetRewardsRange(AvailableRewardsPool.Load(), startIndex, count);

        public static void LogRandomRewards() => AvailableRewardsPool.Log();

        private static void ClearRandomRewards() => AvailableRewardsPool.Clear();

        #endregion

        #region Pending Loot Reward

        public static void SetPendingLootReward(int startIndex) =>
            PlayerPrefs.SetInt(PlayerPrefsKeys.PendingLootRewardIndex, startIndex);

        public static bool TryGetPendingLootReward(out int startIndex)
        {
            if (PlayerPrefs.HasKey(PlayerPrefsKeys.PendingLootRewardIndex))
            {
                startIndex = PlayerPrefs.GetInt(PlayerPrefsKeys.PendingLootRewardIndex);
                return true;
            }

            startIndex = 0;
            return false;
        }

        public static void ClearPendingLootReward() => PlayerPrefs.DeleteKey(PlayerPrefsKeys.PendingLootRewardIndex);

        #endregion
    }
}

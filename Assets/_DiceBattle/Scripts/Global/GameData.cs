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


        public static int CompletedLevels => PlayerPrefs.GetInt(PlayerPrefsKeys.CompletedLevels, 0);
        public static int CurrentLevel => PlayerPrefs.GetInt(PlayerPrefsKeys.CurrentLevel, 0);

        public static void ResetAll()
        {
            ResetCompletedLevels();
            ResetCurrentLevel();

            Inventory.Clear();
            ClearRandomRewards();
            ResetSelectedCharacterClass();
        }

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

        public static void LogRandomRewards() => AvailableRewardsPool.Log();

        private static void ClearRandomRewards() => AvailableRewardsPool.Clear();

        #endregion
    }
}

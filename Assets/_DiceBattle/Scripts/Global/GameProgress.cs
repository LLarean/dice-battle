using System.Linq;
using DiceBattle.UI;
using UnityEngine;

namespace DiceBattle.Global
{
    public static class GameProgress
    {
        public static int CompletedLevels => PlayerPrefs.GetInt(PlayerPrefsKeys.CompletedLevels, 0);
        public static int CurrentLevel => PlayerPrefs.GetInt(PlayerPrefsKeys.CurrentLevel, 0);

        public static void ResetAll()
        {
            ResetCompletedLevels();
            ResetCurrentLevel();

            ClearReceivedRewards();
            ClearRandomRewards();
        }

        public static void IncrementLevels()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.CompletedLevels, CompletedLevels + 1);
            PlayerPrefs.SetInt(PlayerPrefsKeys.CurrentLevel, CurrentLevel + 1);
        }

        public static void IncrementCompletedLevels() => PlayerPrefs.SetInt(PlayerPrefsKeys.CompletedLevels, CompletedLevels + 1);

        public static void ResetCompletedLevels() => PlayerPrefs.DeleteKey(PlayerPrefsKeys.CompletedLevels);

        public static void IncrementCurrentLevel() => PlayerPrefs.SetInt(PlayerPrefsKeys.CurrentLevel, CurrentLevel + 1);

        public static void ResetCurrentLevel() => PlayerPrefs.DeleteKey(PlayerPrefsKeys.CurrentLevel);

        public static int GetDiceCount()
        {
            RewardsData rewardsData = LoadReceivedRewards();

            int firstDice = rewardsData.DiceTypes.Where(rewardType => rewardType == DiceType.AdditionalDice).Sum(rewardType => 1);
            // int secondDice = rewards.RewardTypes.Where(rewardType => rewardType == RewardType.SecondAdditionalDice).Sum(rewardType => 1);

            int diceCount = 3;

            if (firstDice == 1)
            {
                diceCount++;
            }

            // if (secondDice == 1)
            // {
            //     diceCount++;
            // }

            return diceCount;
        }

        #region Game Data

        public static void LoadGameModel() => GameData.Load();

        public static void SaveGameModel() => GameData.Save(null);

        public static void ClearGameModel() => GameData.Clear();

        #endregion

        #region Received Rewards

        public static RewardsData LoadReceivedRewards() => AcquiredRewardsStorage.Load();

        public static void SaveReceivedReward(DiceType diceType) => AcquiredRewardsStorage.Save(diceType);

        public static void LogReceivedReward() => AcquiredRewardsStorage.Log();

        private static void ClearReceivedRewards() => AcquiredRewardsStorage.Clear();

        #endregion

        #region Random Rewards

        public static RewardsData LoadRandomRewards() => AvailableRewardsPool.Load();

        public static void SaveRandomRewards(RewardsData rewardsData) => AvailableRewardsPool.Save(rewardsData);

        public static void LogRandomRewards() => AvailableRewardsPool.Log();

        private static void ClearRandomRewards() => AvailableRewardsPool.Clear();

        #endregion
    }
}

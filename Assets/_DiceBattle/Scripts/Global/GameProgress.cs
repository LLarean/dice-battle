using System.Collections.Generic;
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
            ClearRandomRewards();
            ResetReceivedRewards();
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
            RewardsData rewardsData = GetReceivedRewards();

            int firstDice = rewardsData.RewardTypes.Where(rewardType => rewardType == RewardType.FirstAdditionalDice).Sum(rewardType => 1);
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





        public static void AddReceivedReward(RewardType rewardType)
        {
            string rewardsJson = PlayerPrefs.GetString(PlayerPrefsKeys.ReceivedRewards, "{}");
            RewardsData rewardsData = JsonUtility.FromJson<RewardsData>(rewardsJson) ?? new RewardsData();

            rewardsData.RewardTypes ??= new List<RewardType>();
            rewardsData.RewardTypes.Add(rewardType);

            PlayerPrefs.SetString(PlayerPrefsKeys.ReceivedRewards, JsonUtility.ToJson(rewardsData));

            rewardsJson = PlayerPrefs.GetString(PlayerPrefsKeys.ReceivedRewards, "{}");
            Debug.Log("<color=yellow>ReceivedRewards:</color>" + rewardsJson);
        }

        public static RewardsData GetReceivedRewards()
        {
            string rewardsJson = PlayerPrefs.GetString(PlayerPrefsKeys.ReceivedRewards, "{}");
            RewardsData rewardsData = JsonUtility.FromJson<RewardsData>(rewardsJson) ?? new RewardsData();

            rewardsData.RewardTypes ??= new List<RewardType>();
            return rewardsData;
        }

        public static void ResetReceivedRewards() => PlayerPrefs.DeleteKey(PlayerPrefsKeys.ReceivedRewards);

        #region Random Rewards

        public static void AddRandomRewards(RewardsData rewardsData) => RandomRewards.Save(rewardsData);

        public static RewardsData GetRewardList() => RandomRewards.Load();

        private static void ClearRandomRewards() => RandomRewards.Clear();

        #endregion
    }
}

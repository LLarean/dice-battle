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
            ResetRewards();
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

        public static void AddRewardItem(RewardType rewardType)
        {
            string rewardsJson = PlayerPrefs.GetString(PlayerPrefsKeys.Rewards, "{}");
            Rewards rewards = JsonUtility.FromJson<Rewards>(rewardsJson) ?? new Rewards();

            rewards.RewardTypes ??= new List<RewardType>();
            rewards.RewardTypes.Add(rewardType);

            PlayerPrefs.SetString(PlayerPrefsKeys.Rewards, JsonUtility.ToJson(rewards));
        }

        public static int GetDiceCount()
        {
            Rewards rewards = GetRewards();

            int firstDice = rewards.RewardTypes.Where(rewardType => rewardType == RewardType.FirstAdditionalDice).Sum(rewardType => 1);
            int secondDice = rewards.RewardTypes.Where(rewardType => rewardType == RewardType.SecondAdditionalDice).Sum(rewardType => 1);

            int diceCount = 3;

            if (firstDice == 1)
            {
                diceCount++;
            }

            if (secondDice == 1)
            {
                diceCount++;
            }

            return diceCount;
        }

        public static Rewards GetRewards()
        {
            string rewardsJson = PlayerPrefs.GetString(PlayerPrefsKeys.Rewards, "{}");
            Rewards rewards = JsonUtility.FromJson<Rewards>(rewardsJson) ?? new Rewards();

            rewards.RewardTypes ??= new List<RewardType>();
            return rewards;
        }

        public static void ResetRewards() => PlayerPrefs.DeleteKey(PlayerPrefsKeys.Rewards);
    }
}

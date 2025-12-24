using System.Collections.Generic;
using DiceBattle.Global;
using UnityEngine;

namespace DiceBattle
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

        public static Rewards GetRewards()
        {
            string rewardsJson = PlayerPrefs.GetString(PlayerPrefsKeys.Rewards, "{}");
            Rewards rewards = JsonUtility.FromJson<Rewards>(rewardsJson) ?? new Rewards();

            rewards.RewardTypes ??= new List<RewardType>();
            return rewards;

            // var rewardTypes = new List<RewardType>();

            // if (PlayerPrefs.GetInt(nameof(RewardType.DisableEmptyState), 0) == 1)
            // {
            //     rewardTypes.Add(RewardType.DisableEmptyState);
            // }

            // if (GetValue(RewardType.DisableEmptyState) == 1)
            // {
            //     rewardTypes.Add(RewardType.DisableEmptyState);
            // }
            // if (GetValue(RewardType.FirstAdditionalDice) == 1)
            // {
            //     rewardTypes.Add(RewardType.FirstAdditionalDice);
            // }
            // if (GetValue(RewardType.SecondAdditionalDice) == 1)
            // {
            //     rewardTypes.Add(RewardType.SecondAdditionalDice);
            // }
            // if (GetValue(RewardType.DoubleDamage) == 1)
            // {
            //     rewardTypes.Add(RewardType.DoubleDamage);
            // }
            // if (GetValue(RewardType.DoubleHealth) == 1)
            // {
            //     rewardTypes.Add(RewardType.DoubleHealth);
            // }
            // if (GetValue(RewardType.Armor) == 1)
            // {
            //     rewardTypes.Add(RewardType.Armor);
            // }
            //
            // return rewardTypes;
        }

        public static void ResetRewards() => PlayerPrefs.DeleteKey(PlayerPrefsKeys.Rewards);
    }
}

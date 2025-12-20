using System.Collections.Generic;
using UnityEngine;

namespace DiceBattle
{
    public static class GameProgress
    {
        private static string _currentLevelKey = "CurrentLevel";
        private static string _rewardsKey = "Rewards";

        public static int CurrentLevel => PlayerPrefs.GetInt(_currentLevelKey, 0);

        public static void ResetAll()
        {
            ResetCurrentLevel();
            ResetRewards();
        }

        public static void IncrementCurrentLevel() => PlayerPrefs.SetInt(_currentLevelKey, CurrentLevel + 1);

        public static void ResetCurrentLevel() => PlayerPrefs.DeleteKey(_currentLevelKey);

        public static void AddRewardItem(RewardType rewardType)
        {
            string rewardsJson = PlayerPrefs.GetString(_rewardsKey, "{}");
            Rewards rewards = JsonUtility.FromJson<Rewards>(rewardsJson) ?? new Rewards();

            rewards.RewardTypes ??= new List<RewardType>();
            rewards.RewardTypes.Add(rewardType);

            PlayerPrefs.SetString(_rewardsKey, JsonUtility.ToJson(rewards));
        }

        public static Rewards GetRewards()
        {
            string rewardsJson = PlayerPrefs.GetString(_rewardsKey, "{}");
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

        public static void ResetRewards() => PlayerPrefs.DeleteKey(_rewardsKey);
    }
}

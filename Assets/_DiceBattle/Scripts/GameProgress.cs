using System;
using System.Collections.Generic;
using UnityEngine;

namespace DiceBattle
{
    public static class GameProgress
    {
        private static string _currentLevelKey = "CurrentLevel";

        public static int CurrentLevel => PlayerPrefs.GetInt(_currentLevelKey, 0);

        public static void IncrementCurrentLevel() => PlayerPrefs.SetInt(_currentLevelKey, CurrentLevel + 1);

        public static void ResetCurrentLevel() => PlayerPrefs.SetInt(_currentLevelKey, 0);

        public static void SetRewardItem(RewardType rewardType) => PlayerPrefs.SetInt(rewardType.ToString(), 1);

        public static List<RewardType> GetRewardTypes()
        {
            var rewardTypes = new List<RewardType>();

            // if (PlayerPrefs.GetInt(nameof(RewardType.DisableEmptyState), 0) == 1)
            // {
            //     rewardTypes.Add(RewardType.DisableEmptyState);
            // }

            if (GetValue(RewardType.DisableEmptyState) == 1)
            {
                rewardTypes.Add(RewardType.DisableEmptyState);
            }
            if (GetValue(RewardType.FirstAdditionalDice) == 1)
            {
                rewardTypes.Add(RewardType.FirstAdditionalDice);
            }
            if (GetValue(RewardType.SecondAdditionalDice) == 1)
            {
                rewardTypes.Add(RewardType.SecondAdditionalDice);
            }
            if (GetValue(RewardType.DoubleDamage) == 1)
            {
                rewardTypes.Add(RewardType.DoubleDamage);
            }
            if (GetValue(RewardType.DoubleHealth) == 1)
            {
                rewardTypes.Add(RewardType.DoubleHealth);
            }
            if (GetValue(RewardType.Armor) == 1)
            {
                rewardTypes.Add(RewardType.Armor);
            }

            return rewardTypes;
        }

        private static int GetValue(RewardType key)
        {
            return PlayerPrefs.GetInt(nameof(key), 0);
        }
    }

    public class Rewards
    {
        public List<RewardType> Reward;
    }
}

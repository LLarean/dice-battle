using System;
using System.Collections.Generic;
using System.Linq;
using DiceBattle.UI;
using UnityEngine;
using Random = System.Random;

namespace DiceBattle.Global
{
    public static class RewardsLoader
    {
        public static RewardsData Load(string playerPrefsKey)
        {
            string json = PlayerPrefs.GetString(playerPrefsKey, null);

            if (string.IsNullOrEmpty(json))
            {
                return GetRandomRewards();
            }

            RewardsData data = JsonUtility.FromJson<RewardsData>(json);

            if (data == null)
            {
                Debug.LogWarning("Failed to deserialize rewards data, creating new");
                return GetRandomRewards();
            }

            data.DiceTypes ??= GetRandomDice();
            return data;
        }

        public static void Save(string playerPrefsKey, RewardsData rewardsData)
        {
            if (rewardsData == null)
            {
                Debug.LogWarning("Attempted to save null rewards data");
                return;
            }

            rewardsData.DiceTypes ??= GetRandomDice();
            string json = JsonUtility.ToJson(rewardsData);
            PlayerPrefs.SetString(playerPrefsKey, json);
            PlayerPrefs.Save();
        }

        public static void Log(string playerPrefsKey)
        {
            string randomRewardsJson = PlayerPrefs.GetString(playerPrefsKey, "{}");
            Debug.Log("<color=yellow>RewardList: </color>" + randomRewardsJson);
        }

        public static void Clear(string playerPrefsKey) => PlayerPrefs.DeleteKey(playerPrefsKey);

        private static RewardsData GetRandomRewards()
        {
            return new RewardsData
            {
                DiceTypes = GetRandomDice()
            };
        }

        private static List<DiceType> GetRandomDice()
        {
            var random = new Random();
            var rewardTypes = Enum.GetValues(typeof(DiceType)).Cast<DiceType>().ToList();
            return rewardTypes.OrderBy(x => random.Next()).ToList();
        }
    }
}

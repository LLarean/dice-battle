using System.Collections.Generic;
using DiceBattle.UI;
using UnityEngine;

namespace DiceBattle.Global
{
    public static class RandomRewards
    {
        private const string _playerPrefsKey = PlayerPrefsKeys.RewardsList;

        public static void Save(RewardsData rewardsData)
        {
            if (rewardsData == null)
            {
                Debug.LogWarning("Attempted to save null rewards data");
                return;
            }

            rewardsData.RewardTypes ??= new List<RewardType>();
            string json = JsonUtility.ToJson(rewardsData);
            PlayerPrefs.SetString(_playerPrefsKey, json);
            PlayerPrefs.Save();
        }

        public static RewardsData Load()
        {
            string json = PlayerPrefs.GetString(_playerPrefsKey, null);

            if (string.IsNullOrEmpty(json))
            {
                return CreateNewRewardsData();
            }

            RewardsData data = JsonUtility.FromJson<RewardsData>(json);

            if (data == null)
            {
                Debug.LogWarning("Failed to deserialize rewards data, creating new");
                return CreateNewRewardsData();
            }

            data.RewardTypes ??= new List<RewardType>();
            return data;
        }

        public static void Clear() => PlayerPrefs.DeleteKey(_playerPrefsKey);

        private static RewardsData CreateNewRewardsData()
        {
            return new RewardsData
            {
                RewardTypes = new List<RewardType>()
            };
        }
    }
}

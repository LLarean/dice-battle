using System.Collections.Generic;
using DiceBattle.UI;
using UnityEngine;

namespace DiceBattle.Global
{
    public static class AcquiredRewardsStorage
    {
        private const string _playerPrefsKey = PlayerPrefsKeys.ReceivedRewards;

        public static void Save(DiceType diceType)
        {
            string rewardsJson = PlayerPrefs.GetString(PlayerPrefsKeys.ReceivedRewards, null);

            RewardsData rewardsData = string.IsNullOrEmpty(rewardsJson)
                ? CreateNewRewardsData()
                : JsonUtility.FromJson<RewardsData>(rewardsJson);

            rewardsData ??= CreateNewRewardsData();
            rewardsData.DiceTypes.Add(diceType);

            PlayerPrefs.SetString(_playerPrefsKey, JsonUtility.ToJson(rewardsData));
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

            return data;
        }

        public static void Log()
        {
            string rewardsJson = PlayerPrefs.GetString(PlayerPrefsKeys.ReceivedRewards, "{}");
            Debug.Log("<color=yellow>AcquiredRewardsStorage:</color>" + rewardsJson);
        }

        public static void Clear() => PlayerPrefs.DeleteKey(_playerPrefsKey);

        private static RewardsData CreateNewRewardsData()
        {
            return new RewardsData
            {
                DiceTypes = new List<DiceType>()
            };
        }
    }
}

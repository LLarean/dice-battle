using System.Collections.Generic;
using DiceBattle.UI;
using UnityEngine;

namespace DiceBattle.Global
{
    public static class AcquiredRewardsStorage
    {
        private const string _playerPrefsKey = PlayerPrefsKeys.PlayerInventory;

        public static DiceList Load()
        {
            string json = PlayerPrefs.GetString(_playerPrefsKey, null);

            if (string.IsNullOrEmpty(json))
            {
                return CreateNewRewardsData();
            }

            DiceList data = JsonUtility.FromJson<DiceList>(json);

            if (data == null)
            {
                Debug.LogWarning("Failed to deserialize rewards data, creating new");
                return CreateNewRewardsData();
            }

            return data;
        }

        public static void Save(DiceType diceType)
        {
            string rewardsJson = PlayerPrefs.GetString(PlayerPrefsKeys.PlayerInventory, null);

            DiceList diceList = string.IsNullOrEmpty(rewardsJson)
                ? CreateNewRewardsData()
                : JsonUtility.FromJson<DiceList>(rewardsJson);

            diceList ??= CreateNewRewardsData();
            diceList.DiceTypes.Add(diceType);

            PlayerPrefs.SetString(_playerPrefsKey, JsonUtility.ToJson(diceList));
            PlayerPrefs.Save();
        }

        public static void Log()
        {
            string rewardsJson = PlayerPrefs.GetString(PlayerPrefsKeys.PlayerInventory, "{}");
            Debug.Log("<color=yellow>AcquiredRewardsStorage:</color>" + rewardsJson);
        }

        public static void Clear() => PlayerPrefs.DeleteKey(_playerPrefsKey);

        private static DiceList CreateNewRewardsData()
        {
            return new DiceList
            {
                DiceTypes = new List<DiceType>()
            };
        }
    }
}

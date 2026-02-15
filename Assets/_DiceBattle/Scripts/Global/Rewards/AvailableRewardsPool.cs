using System;
using System.Collections.Generic;
using System.Linq;
using DiceBattle.UI;
using UnityEngine;
using Random = System.Random;

namespace DiceBattle.Global
{
    public static class AvailableRewardsPool
    {
        private const string _playerPrefsKey = PlayerPrefsKeys.RewardsList;

        public static DiceList Load()
        {
            string json = PlayerPrefs.GetString(_playerPrefsKey, null);

            if (string.IsNullOrEmpty(json))
            {
                return GetRandomRewards();
            }

            DiceList data = JsonUtility.FromJson<DiceList>(json);

            if (data == null)
            {
                Debug.LogWarning("Failed to deserialize rewards data, creating new");
                return GetRandomRewards();
            }

            data.DiceTypes ??= GetRandomDice();
            return data;
        }

        public static void Save(DiceList diceList)
        {
            if (diceList == null)
            {
                Debug.LogWarning("Attempted to save null rewards data");
                return;
            }

            diceList.DiceTypes ??= GetRandomDice();
            string json = JsonUtility.ToJson(diceList);
            PlayerPrefs.SetString(_playerPrefsKey, json);
            PlayerPrefs.Save();
        }

        public static void Log()
        {
            string randomRewardsJson = PlayerPrefs.GetString(_playerPrefsKey, "{}");
            Debug.Log("<color=yellow>AvailableRewardsPool: </color>" + randomRewardsJson);
        }

        public static void Clear() => PlayerPrefs.DeleteKey(_playerPrefsKey);

        private static DiceList GetRandomRewards()
        {
            return new DiceList
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

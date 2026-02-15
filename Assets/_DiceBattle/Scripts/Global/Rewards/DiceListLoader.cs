using System.Collections.Generic;
using DiceBattle.UI;
using UnityEngine;

namespace DiceBattle.Global
{
    public static class DiceListLoader
    {
        public static DiceList Load(string playerPrefsKey)
        {
            string json = PlayerPrefs.GetString(playerPrefsKey, null);

            if (string.IsNullOrEmpty(json))
            {
                return GetRewards();
            }

            DiceList diceList = JsonUtility.FromJson<DiceList>(json);

            if (diceList == null)
            {
                Debug.LogWarning("Failed to deserialize rewards data, creating new");
                return GetRewards();
            }

            diceList.DiceTypes ??= GetDice();
            return diceList;
        }

        public static void Save(string playerPrefsKey, DiceList diceList)
        {
            if (diceList == null)
            {
                Debug.LogWarning("Attempted to save null rewards data");
                return;
            }

            diceList.DiceTypes ??= GetDice();
            string json = JsonUtility.ToJson(diceList);
            PlayerPrefs.SetString(playerPrefsKey, json);
            PlayerPrefs.Save();
        }

        public static void Log(string playerPrefsKey)
        {
            string diceListJson = PlayerPrefs.GetString(playerPrefsKey, "{}");
            Debug.Log("<color=yellow>DiceList: </color>" + diceListJson);
        }

        public static void Clear(string playerPrefsKey) => PlayerPrefs.DeleteKey(playerPrefsKey);

        private static DiceList GetRewards()
        {
            return new DiceList
            {
                DiceTypes = GetDice(),
            };
        }

        private static List<DiceType> GetDice()
        {
            return new List<DiceType>();
        }
    }
}

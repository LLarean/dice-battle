using System.Collections.Generic;
using UnityEngine;

namespace DiceBattle.UI
{
    public static class ItemsStorage
    {
        public static List<Item> Load(string playerPrefsKey)
        {
            string json = PlayerPrefs.GetString(playerPrefsKey, null);

            if (string.IsNullOrEmpty(json))
            {
                return new List<Item>();
            }

            List<Item> equippedItems = JsonUtility.FromJson<List<Item>>(json);

            return equippedItems ?? new List<Item>();
        }

        public static void Save(string playerPrefsKey, List<Item> updatedItems)
        {
            string itemsJson = JsonUtility.ToJson(updatedItems);
            PlayerPrefs.SetString(playerPrefsKey, itemsJson);
            PlayerPrefs.Save();
        }

        public static void Reset(string playerPrefsKey)
        {
            PlayerPrefs.DeleteKey(playerPrefsKey);
        }
    }
}

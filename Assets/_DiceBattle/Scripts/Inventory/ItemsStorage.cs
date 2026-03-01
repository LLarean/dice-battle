using System.Collections.Generic;
using UnityEngine;

namespace DiceBattle.UI
{
    public static class ItemsStorage
    {
        [System.Serializable]
        private class ItemList
        {
            public List<Item> Items;
        }

        public static List<Item> Load(string playerPrefsKey)
        {
            string json = PlayerPrefs.GetString(playerPrefsKey, null);

            if (string.IsNullOrEmpty(json))
            {
                return new List<Item>();
            }

            return JsonUtility.FromJson<ItemList>(json)?.Items ?? new List<Item>();
        }

        public static void Save(string playerPrefsKey, List<Item> updatedItems)
        {
            string itemsJson = JsonUtility.ToJson(new ItemList { Items = updatedItems });
            PlayerPrefs.SetString(playerPrefsKey, itemsJson);
            PlayerPrefs.Save();
        }

        public static void Reset(string playerPrefsKey)
        {
            PlayerPrefs.DeleteKey(playerPrefsKey);
        }
    }
}

using System;
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

            List<Item> items = JsonUtility.FromJson<ItemList>(json)?.Items ?? new List<Item>();
            ReplaceRemovedTypes(items);
            return items;
        }

        // Saves from before the dice rework may hold removed types; keep the items so index-based IDs stay unique.
        private static void ReplaceRemovedTypes(List<Item> items)
        {
            foreach (Item item in items)
            {
                if (Enum.IsDefined(typeof(DiceType), item.Type) == false)
                {
                    item.Type = DiceType.Default;
                }
            }
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

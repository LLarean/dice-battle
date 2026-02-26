using System.Collections.Generic;
using UnityEngine;

namespace DiceBattle.UI
{
    public record Items
    {
        private readonly string _playerPrefsKey;

        public Items(string playerPrefsKey)
        {
            _playerPrefsKey = playerPrefsKey;
        }

        public List<Item> Value()
        {
            string json = PlayerPrefs.GetString(_playerPrefsKey, null);

            if (string.IsNullOrEmpty(json))
            {
                return new List<Item>();
            }

            List<Item> equippedItems = JsonUtility.FromJson<List<Item>>(json);

            return equippedItems ?? new List<Item>();
        }
    }
}

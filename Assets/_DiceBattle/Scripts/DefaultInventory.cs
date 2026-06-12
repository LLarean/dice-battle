using System.Collections.Generic;
using DiceBattle.Data;
using DiceBattle.Global;
using DiceBattle.UI;
using UnityEngine;

namespace DiceBattle
{
    public class DefaultInventory : MonoBehaviour
    {
        [SerializeField] private GameConfig _gameConfig;

        public void SetDefaultInventory()
        {
            if (Inventory.AllItems().Count != 0)
                return;

            InitializeDefault(_gameConfig.DiceStartCount);
        }

        public static void InitializeDefault(int diceCount)
        {
            var items = new List<Item>();
            for (int i = 0; i < diceCount; i++)
            {
                items.Add(new Item
                {
                    ID = i.ToString(),
                    Type = DiceType.Default,
                    IsEquipped = true,
                });
            }
            Inventory.AddItems(items);
        }
    }
}

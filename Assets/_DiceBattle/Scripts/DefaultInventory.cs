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
            List<Item> allItems = Inventory.AllItems();

            if (allItems.Count != 0)
            {
                return;
            }

            allItems.AddRange(_gameConfig.DefaultInventory);
            Inventory.AddItems(allItems);
        }
    }
}

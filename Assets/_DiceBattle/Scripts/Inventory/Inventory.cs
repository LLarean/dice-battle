using System.Collections.Generic;
using DiceBattle.Global;

namespace DiceBattle.UI
{
    public static class Inventory
    {
        public static List<Item> AllItems()
        {
            return ItemsStorage.Load(PlayerPrefsKeys.AllItems);
        }

        public static List<Item> EquippedItems()
        {
            return ItemsStorage.Load(PlayerPrefsKeys.AllItems).FindAll(i => i.IsEquipped);
        }

        public static List<Item> UnequippedItems()
        {
            return ItemsStorage.Load(PlayerPrefsKeys.AllItems).FindAll(i => i.IsEquipped == false);
        }

        public static void AddItemToUnequipped(Item item)
        {
            List<Item> allItems = ItemsStorage.Load(PlayerPrefsKeys.AllItems);
            item.ID = allItems.Count.ToString();
            allItems.Add(item);
            ItemsStorage.Save(PlayerPrefsKeys.AllItems, allItems);
        }

        public static void EquipItem(Item item)
        {
            List<Item> allItems = AllItems();
            Item itemForEquipped = allItems.Find(i => i.ID == item.ID);
            itemForEquipped.IsEquipped = true;
            ItemsStorage.Save(PlayerPrefsKeys.AllItems, allItems);
        }

        public static void UnequipItem(Item item)
        {
            List<Item> allItems = AllItems();
            Item itemForEquipped = allItems.Find(i => i.ID == item.ID);
            itemForEquipped.IsEquipped = false;
            ItemsStorage.Save(PlayerPrefsKeys.AllItems, allItems);
        }

        public static void Clear()
        {
            ItemsStorage.Reset(PlayerPrefsKeys.AllItems);
        }
    }
}

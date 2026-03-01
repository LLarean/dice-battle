using System.Collections.Generic;
using DiceBattle.Global;

namespace DiceBattle.UI
{
    public record Inventory
    {
        private const string _equippedItemsKey = "EquippedItems";
        private const string _unequippedItemsKey = "UnequippedItems";

        public List<Item> AllItems()
        {
            return new Items(PlayerPrefsKeys.AllItems).Value();
        }

        public void AddItem(Item item)
        {
            List<Item> allItems = AllItems();
            item.ID = allItems.Count.ToString();
            allItems.Add(item);
        }

        public List<Item> EquippedItems()
        {
            List<Item> allItems = AllItems();
            return allItems.FindAll(i => i.IsEquipped);
        }

        public List<Item> UnequippedItems()
        {
            List<Item> allItems = AllItems();
            return allItems.FindAll(i => i.IsEquipped == false);
        }

        public void EquipItem(Item item)
        {
            List<Item> allItems = AllItems();
            Item itemForEquipped = allItems.Find(i => i.ID == item.ID);
            itemForEquipped.IsEquipped = true;
        }

        public void UnequipItem(Item item)
        {
            List<Item> allItems = AllItems();
            Item itemForUnequipped = allItems.Find(i => i.ID == item.ID);
            itemForUnequipped.IsEquipped = true;
        }

        public void AddEquippedItem(Item item)
        {
            // Check id???
            List<Item> equippedItems = EquippedItems();
            equippedItems.Add(item);
        }

        public bool CanEquipItem(Item item)
        {
            List<Item> equippedItems = EquippedItems();
            return equippedItems.Contains(item);
        }

        public bool CanAddUnequippedItem(Item item)
        {
            List<Item> unquippedItems = UnequippedItems();
            return unquippedItems.Contains(item);
        }

        public void AddUnequippedItem(Item item)
        {
            List<Item> unequippedItems = UnequippedItems();
            unequippedItems.Add(item);
        }

        public void Clear()
        {
            new Items(PlayerPrefsKeys.AllItems).Reset();
            new Items(_equippedItemsKey).Reset();
            new Items(_unequippedItemsKey).Reset();
        }
    }
}

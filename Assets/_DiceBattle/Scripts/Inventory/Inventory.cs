using System.Collections.Generic;
using DiceBattle.Global;

namespace DiceBattle.UI
{
    public record Inventory
    {
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

        public void Clear()
        {
            new Items(PlayerPrefsKeys.AllItems).Reset();
        }
    }
}

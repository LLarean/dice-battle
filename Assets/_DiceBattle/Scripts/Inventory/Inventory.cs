using System.Collections.Generic;

namespace DiceBattle.UI
{
    public record Inventory
    {
        private const string _equippedItemsKey = "EquippedItems";
        private const string _unequippedItemsKey = "UnequippedItems";

        public List<Item> EquippedItems()
        {
            return new Items(_equippedItemsKey).Value();
        }

        public List<Item> UnequippedItems()
        {
            return new Items(_unequippedItemsKey).Value();
        }

        public void EquipItem(Item item)
        {
            List<Item> equippedItems = EquippedItems();
            List<Item> unequippedItems = UnequippedItems();
            unequippedItems.Remove(item);
            equippedItems.Add(item);
        }

        public void AddEquippedItem(Item item)
        {
            List<Item> equippedItems = EquippedItems();
            equippedItems.Add(item);
        }

        public void AddUnequippedItem(Item item)
        {
            List<Item> unequippedItems = UnequippedItems();
            unequippedItems.Add(item);
        }

        public void Clear()
        {
            new Items(_equippedItemsKey).Reset();
            new Items(_unequippedItemsKey).Reset();
        }
    }
}

using System.Collections.Generic;

namespace DiceBattle.UI
{
    public record Inventory
    {
        public List<Item> GetEquippedItems()
        {
            return new Items("EquippedItems").Value();
        }

        public List<Item> GetUnequippedItems()
        {
            return new Items("UnequippedItems").Value();
        }

        public void EquipItem(Item item)
        {
            List<Item> equippedItems = GetEquippedItems();
            List<Item> unequippedItems = GetUnequippedItems();
            unequippedItems.Remove(item);
            equippedItems.Add(item);
        }

        public void AddEquippedItem(Item item)
        {
            List<Item> equippedItems = GetEquippedItems();
            equippedItems.Add(item);
        }

        public void AddUnequippedItem(Item item)
        {
            List<Item> unequippedItems = GetUnequippedItems();
            unequippedItems.Add(item);
        }
    }
}

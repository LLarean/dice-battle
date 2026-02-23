using System.Linq;
using DiceBattle.Global;

namespace DiceBattle.UI
{
    public class Inventory
    {
        public DiceList GetEquippedItems() => DiceListLoader.Load(PlayerPrefsKeys.EquippedRewards);

        public int GetItemsCount(DiceType diceType) => GetEquippedItems().DiceTypes.Count(r => r == diceType);

        public void SaveEquippedRewards(DiceList diceList) => DiceListLoader.Save(PlayerPrefsKeys.EquippedRewards, diceList);

        public static void LogEquippedRewards() => DiceListLoader.Log("Equipped", PlayerPrefsKeys.EquippedRewards);

        private static void ClearEquippedRewards() => DiceListLoader.Clear(PlayerPrefsKeys.EquippedRewards);
    }

    public class Item
    {
        public string ID;
        public DiceType Type;
    }
}

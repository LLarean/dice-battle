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
            DiceList inventory = GameData.GetInventory();

            if (inventory.DiceTypes.Count == 0)
            {
                inventory.DiceTypes.AddRange(_gameConfig.DefaultInventory.DiceTypes);
                GameData.UpdateInventory(inventory);
            }

            GameData.LogInventory();
        }

        public void SetEquippedItems()
        {
            DiceList equippedItems = GameData.GetEquippedItems();

            if (equippedItems.DiceTypes.Count == 0)
            {
                DiceList inventory = GameData.GetInventory();
                equippedItems.DiceTypes.AddRange(inventory.DiceTypes);

                GameData.SaveEquippedRewards(equippedItems);
                GameData.LogEquippedRewards();
            }
        }
    }
}

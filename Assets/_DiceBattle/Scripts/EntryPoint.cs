using DiceBattle.Data;
using DiceBattle.Events;
using DiceBattle.Global;
using DiceBattle.UI;
using GameSignals;
using UnityEngine;

namespace DiceBattle
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameConfig _gameConfig;

        private void Start()
        {
            Application.targetFrameRate = 60;
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.MainMenu));

            SetDefaultInventory();
            SetEquippedItems();
        }

        private void SetDefaultInventory()
        {
            DiceList inventory = GameData.GetInventory();

            if (inventory.DiceTypes.Count == 0)
            {
                inventory.DiceTypes.AddRange(_gameConfig.DefaultInventory.DiceTypes);
            }

            GameData.UpdateInventory(inventory);
            GameData.LogInventory();
        }

        private void SetEquippedItems()
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

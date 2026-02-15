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
        }

        private void SetDefaultInventory()
        {
            DiceList inventory = GameData.GetInventory();

            if (inventory.DiceTypes.Count == 0)
            {
                inventory.DiceTypes.AddRange(_gameConfig.DefaultInventory.DiceTypes);
            }

            GameData.SaveInventory(inventory);
            GameData.LogInventory();
        }
    }
}

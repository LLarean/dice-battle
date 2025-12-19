using System.Collections.Generic;
using DiceBattle.Audio;
using GameSignals;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.Screens
{
    public class DungeonsScreen : MonoBehaviour
    {
        [SerializeField] private List<LevelItem> _levelItems;
        [SerializeField] private Button _restart;
        [SerializeField] private Button _inventory;

        private void Start()
        {
            foreach (LevelItem levelItem in _levelItems)
            {
                levelItem.OnClicked += OpenLevel;
            }

            _restart.onClick.AddListener(RestartGame);
            _inventory.onClick.AddListener(ShowInventory);
        }

        private void OnDestroy()
        {
            foreach (LevelItem levelItem in _levelItems)
            {
                levelItem.OnClicked -= OpenLevel;
            }

            _restart.onClick.RemoveAllListeners();
            _inventory.onClick.RemoveAllListeners();
        }

        private void OnEnable()
        {
            int currentLevel = GameProgress.CurrentLevel;
            // int availableLevels = PlayerPrefs.GetInt("AvailableLevels", 1);

            for (int i = 0; i < _levelItems.Count; i++)
            {
                if (i <= currentLevel)
                {
                    _levelItems[i].EnableAvailable();
                }
                else
                {
                    _levelItems[i].DisableAvailable();
                }

                if (i == currentLevel)
                {
                    _levelItems[i].DisableAggry();
                }
            }
        }

        private void RestartGame()
        {
            GameProgress.ResetCurrentLevel();
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.MainMenu));
        }

        private void ShowInventory()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.InventoryWindow));
        }

        private void OpenLevel(int levelIndex)
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.GameScreen));
        }
    }
}

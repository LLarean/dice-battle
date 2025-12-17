using System;
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
            SignalSystem.Raise<ITopBarHandler>(handler => handler.Hide());
        }

        private void RestartGame()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.MainMenu));
        }

        private void ShowInventory()
        {
            SignalSystem.Raise<IInventoryWindowHandler>(handler => handler.Show());
        }

        private void OpenLevel(int levelIndex)
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.GameScreen));
        }
    }
}

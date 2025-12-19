using System.Collections.Generic;
using DiceBattle.Data;
using DiceBattle.Events;
using GameSignals;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.Screens
{
    public class DungeonsScreen : MonoBehaviour
    {
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private LevelItem _levelItem;
        [SerializeField] private Transform _levelItemSpawn;
        [Space]
        [SerializeField] private Button _restart;
        [SerializeField] private Button _inventory;

        private List<LevelItem> _levelItems = new();

        private void Start()
        {
            _restart.onClick.AddListener(RestartGame);
            _inventory.onClick.AddListener(ShowInventory);

            _levelItems = new List<LevelItem>();

            for (int i = 0; i < _gameConfig.EnemiesPortraits.Length; i++)
            {
                LevelItem levelItem = Instantiate(_levelItem, _levelItemSpawn);
                levelItem.Initialize(_gameConfig.EnemiesPortraits[i], i + 1);
                levelItem.OnClicked += OpenLevel;
                _levelItems.Add(levelItem);
            }

            UpdateLevelItemsAvailability();
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

        private void OnEnable() => UpdateLevelItemsAvailability();

        private void RestartGame()
        {
            GameProgress.ResetCurrentLevel();
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.MainMenu));
        }

        private void ShowInventory()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.InventoryWindow));
        }

        private void OpenLevel()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.GameScreen));
        }

        private void UpdateLevelItemsAvailability()
        {
            int currentLevel = GameProgress.CurrentLevel;

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
    }
}

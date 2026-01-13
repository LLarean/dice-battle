using System.Collections.Generic;
using DiceBattle.Audio;
using DiceBattle.Core;
using DiceBattle.Data;
using DiceBattle.Events;
using DiceBattle.Global;
using GameSignals;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class DungeonsScreen : Screen
    {
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private LevelItem _levelItem;
        [SerializeField] private Transform _levelItemSpawn;
        [Space]
        [SerializeField] private Button _restart;
        [SerializeField] private Button _inventory;

        private List<LevelItem> _levelItems = new();

        private void CreateLevelItems()
        {
            ClearLevelItems();

            for (int i = 0; i < _gameConfig.Enemy.Portraits.Length; i++)
            {
                LevelData levelData = GetLevelData(i);
                LevelItem levelItem = Instantiate(_levelItem, _levelItemSpawn);

                levelItem.Initialize(levelData);
                levelItem.OnClicked += OpenLevel;

                _levelItems.Add(levelItem);
            }
        }

        private void ClearLevelItems()
        {
            foreach (LevelItem levelItem in _levelItems)
            {
                Destroy(levelItem.gameObject);
            }

            _levelItems = new List<LevelItem>();
        }

        private LevelData GetLevelData(int index)
        {
            int currentLevel = GameProgress.CurrentLevel;
            bool isAvailable = index == currentLevel;
            bool isCompleted = index < currentLevel;

            return new LevelData
            {
                Portrait = _gameConfig.Enemy.Portraits[index],
                Title = $"Уровень {index + 1}", // TODO Translation into other languages
                IsAvailable = isAvailable,
                IsCompleted = isCompleted,
            };
        }

        #region Unity lifecycle

        private void Start()
        {
            _restart.onClick.AddListener(HandleRestartGameClick);
            _inventory.onClick.AddListener(HandleShowInventoryClick);
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
            // SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Tavern));
            CreateLevelItems();
        }

        #endregion

        #region Handlers

        private void HandleRestartGameClick()
        {
            GameProgress.ResetAll();
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.MainMenu));
        }

        private void HandleShowInventoryClick()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.InventoryWindow));
        }

        private void OpenLevel()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.GameScreen));
        }

        #endregion
    }
}

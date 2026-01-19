using DiceBattle.Audio;
using DiceBattle.Core;
using DiceBattle.Data;
using DiceBattle.Events;
using DiceBattle.Global;
using GameSignals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class TavernScreen : Screen
    {
        [SerializeField] private GameConfig _gameConfig;
        [Space]
        [SerializeField] private Button _inventory;
        [Space]
        [SerializeField] private Button _start;
        [SerializeField] private TextMeshProUGUI _startLabel;

        private LevelData GetLevelData(int index)
        {
            int completedLevels = GameProgress.CompletedLevels;
            bool isCompleted = index < completedLevels;
            bool isAvailable = index == completedLevels;

            return new LevelData
            {
                Portrait = _gameConfig.Enemies[index].Portrait,
                Title = $"Уровень {index + 1}", // TODO Translation into other languages
                IsAvailable = isAvailable,
                IsCompleted = isCompleted,
            };
        }

        #region Unity lifecycle

        private void Start()
        {
            _start.onClick.AddListener(HandleStartClick);
            _inventory.onClick.AddListener(HandleInventoryClick);
        }

        private void OnDestroy()
        {
            _start.onClick.RemoveAllListeners();
            _inventory.onClick.RemoveAllListeners();
        }

        private void OnEnable()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Tavern));
        }

        #endregion

        #region Handlers

        private void HandleInventoryClick()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.InventoryWindow));
        }

        private void HandleStartClick()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.GameScreen));
        }

        #endregion
    }
}

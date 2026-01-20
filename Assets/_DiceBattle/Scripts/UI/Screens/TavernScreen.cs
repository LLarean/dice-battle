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
        [Space]
        [SerializeField] private TextMeshProUGUI _message;

        private void ShowMessage()
        {
            int completedLevels = GameProgress.CompletedLevels;

            // TODO Separate it into a separate logic
            // Add translation to other languages
            if (completedLevels == 0)
            {
                _message.text = "Добро пожаловать в таверну!";
            }
            else
            {
                _message.text = "Как ваши приключения?";
            }
        }

        private void SetLabel()
        {
            int completedLevels = GameProgress.CompletedLevels;
            _startLabel.text = "Уровень " + (completedLevels + 1); // TODO Translation into other languages
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
            ShowMessage();
            SetLabel();
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

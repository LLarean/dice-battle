using DiceBattle.Audio;
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
        [Space]
        [SerializeField] private Button _restart;
        [SerializeField] private Button _start;
        [SerializeField] private Button _inventory;
        [SerializeField] private TextMeshProUGUI _startLabel;
        [Space]
        [SerializeField] private Innkeeper _innkeeper;

        private void SetLabel()
        {
            int completedLevels = GameProgress.CompletedLevels;
            _startLabel.text = "Уровень " + (completedLevels + 1); // TODO Translation into other languages
        }

        #region Unity lifecycle

        private void Start()
        {
            _restart.onClick.AddListener(HandleRestartClick);
            _start.onClick.AddListener(HandleStartClick);
            _inventory.onClick.AddListener(HandleInventoryClick);
        }

        private void OnDestroy()
        {
            _restart.onClick.RemoveAllListeners();
            _start.onClick.RemoveAllListeners();
            _inventory.onClick.RemoveAllListeners();
        }

        private void OnEnable()
        {
            _innkeeper.ShowMessage();
            SetLabel();
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlayMusic(SoundType.Tavern));
        }

        #endregion

        #region Handlers

        private void HandleRestartClick()
        {
            GameProgress.ResetAll();
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.MainMenu));
        }

        private void HandleStartClick()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.GameScreen));
        }

        private void HandleInventoryClick()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.InventoryWindow));
        }

        #endregion
    }
}

using System;
using DiceBattle.Events;
using GameSignals;
using UnityEngine;

namespace DiceBattle.UI
{
    public class ScreenChanger : MonoBehaviour, IScreenHandler
    {
        [Header("Screens")]
        [SerializeField] private MainMenuScreen _mainMenuScreen;
        [SerializeField] private TavernScreen _tavernScreen;
        [SerializeField] private GameScreen _gameScreen;
        [SerializeField] private GameOverScreen _gameOverScreen;
        [SerializeField] private LootScreen _lootScreen;
        [Header("Windows")]
        [SerializeField] private OptionsWindow _optionsWindow;
        [SerializeField] private InventoryWindow _inventoryWindow;

        private Screen _currentScreen;

        public void ShowScreen(ScreenType screenType)
        {
            if (_currentScreen != null)
            {
                _currentScreen.Hide();
            }

            Screen nextScreen = GetScreen(screenType);
            nextScreen.Show();
            _currentScreen = nextScreen;
        }

        public void ShowWindow(ScreenType screenType)
        {
            Screen window = GetScreen(screenType);
            window.Show();
        }

        public void Back()
        {
            if (_currentScreen.TryGetComponent(out TavernScreen dungeonsScreen))
            {
                ShowScreen(ScreenType.MainMenu);
            }
            else if (_currentScreen.TryGetComponent(out GameScreen gameScreen))
            {
                ShowScreen(ScreenType.DungeonsScreen);
            }
        }

        private Screen GetScreen(ScreenType screenType)
        {
            return screenType switch {
                ScreenType.MainMenu => _mainMenuScreen,
                ScreenType.GameScreen => _gameScreen,
                ScreenType.GameOverScreen => _gameOverScreen,
                ScreenType.LootScreen => _lootScreen,
                ScreenType.DungeonsScreen => _tavernScreen,

                ScreenType.OptionsWindow => _optionsWindow,
                ScreenType.InventoryWindow => _inventoryWindow,
                _ => throw new ArgumentOutOfRangeException(nameof(screenType), screenType, null)
            };
        }

        private void Awake()
        {
            _mainMenuScreen.gameObject.SetActive(false);
            _tavernScreen.gameObject.SetActive(false);
            _gameOverScreen.gameObject.SetActive(false);
            _gameScreen.gameObject.SetActive(false);
            _lootScreen.gameObject.SetActive(false);

            _optionsWindow.gameObject.SetActive(false);
            _inventoryWindow.gameObject.SetActive(false);

            SignalSystem.Subscribe(this);
        }

        private void OnDestroy() => SignalSystem.Unsubscribe(this);
    }
}

using System;
using DiceBattle.Audio;
using DiceBattle.Windows;
using GameSignals;
using UnityEngine;

namespace DiceBattle.Screens
{
    public class ScreenChanger : MonoBehaviour, IScreenHandler
    {
        [Header("Screens")]
        [SerializeField] private MainMenuScreen _mainMenuScreen;
        [SerializeField] private DungeonsScreen _dungeonsScreen;
        [SerializeField] private GameScreen _gameScreen;
        [SerializeField] private GameOverScreen _gameOverScreen;
        [SerializeField] private LootScreen _lootScreen;
        [Header("Windows")]
        [SerializeField] private OptionsWindow _optionsWindow;
        [SerializeField] private InventoryWindow _inventoryWindow;

        private GameObject _currentScreen;

        public void ShowScreen(ScreenType screenType)
        {
            if (_currentScreen != null)
            {
                _currentScreen.gameObject.SetActive(false);
            }

            GameObject screen = screenType switch {
                ScreenType.MainMenu => _mainMenuScreen.gameObject,
                ScreenType.GameScreen => _gameScreen.gameObject,
                ScreenType.GameOverScreen => _gameOverScreen.gameObject,
                ScreenType.LootScreen => _lootScreen.gameObject,
                ScreenType.DungeonsScreen => _dungeonsScreen.gameObject,
                _ => throw new ArgumentOutOfRangeException(nameof(screenType), screenType, null)
            };

            screen.SetActive(true);
            _currentScreen = screen;
        }

        public void ShowWindow(ScreenType screenType)
        {
            GameObject window = screenType switch {
                ScreenType.OptionsWindow => _optionsWindow.gameObject,
                ScreenType.InventoryWindow => _inventoryWindow.gameObject,
                _ => throw new ArgumentOutOfRangeException(nameof(screenType), screenType, null)
            };

            window.SetActive(true);
        }

        public void Back()
        {
            if (_currentScreen.TryGetComponent(out DungeonsScreen dungeonsScreen))
            {
                ShowScreen(ScreenType.MainMenu);
            }
            else if (_currentScreen.TryGetComponent(out GameScreen gameScreen))
            {
                ShowScreen(ScreenType.DungeonsScreen);
            }
        }

        private void Awake()
        {
            _mainMenuScreen.gameObject.SetActive(false);
            _dungeonsScreen.gameObject.SetActive(false);
            _gameOverScreen.gameObject.SetActive(false);
            _gameScreen.gameObject.SetActive(false);
            _lootScreen.gameObject.SetActive(false);

            SignalSystem.Subscribe(this);
        }

        private void OnDestroy() => SignalSystem.Unsubscribe(this);
    }
}

using System;
using DiceBattle.Audio;
using GameSignals;
using UnityEngine;

namespace DiceBattle.Screens
{
    public class ScreenChanger : MonoBehaviour, IScreenHandler
    {
        [SerializeField] private MainMenuScreen _mainMenuScreen;
        [SerializeField] private DungeonsScreen _dungeonsScreen;
        [SerializeField] private GameScreen _gameScreen;
        [SerializeField] private GameOverScreen _gameOverScreen;
        [SerializeField] private LootScreen _lootScreen;

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

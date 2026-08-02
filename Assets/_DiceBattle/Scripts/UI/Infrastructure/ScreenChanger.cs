using System;
using System.Collections.Generic;
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
        [SerializeField] private InventoryScreen _inventoryScreen;
        [SerializeField] private GameScreen _gameScreen;
        [SerializeField] private GameOverScreen _gameOverScreen;
        [SerializeField] private LootScreen _lootScreen;
        [Header("Windows")]
        [SerializeField] private OptionsWindow _optionsWindow;
        [SerializeField] private CreditsWindow _creditsWindow;
        [SerializeField] private HelpWindow _helpWindow;
        [SerializeField] private QuestWindow _questWindow;
        [SerializeField] private ConfirmWindow _confirmWindow;

        private const float TransitionLockDuration = 0.35f;

        private Screen _currentScreen;
        private readonly Stack<Screen> _openWindows = new();
        private float _transitionLockedUntil;

        private bool IsTransitioning => Time.unscaledTime < _transitionLockedUntil;

        private void LockTransitions() => _transitionLockedUntil = Time.unscaledTime + TransitionLockDuration;

        public void ShowScreen(ScreenType screenType)
        {
            if (IsTransitioning)
            {
                return;
            }

            LockTransitions();

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
            if (IsTransitioning)
            {
                return;
            }

            LockTransitions();

            Screen window = GetScreen(screenType);
            window.Show();
            _openWindows.Push(window);
        }

        public void CloseTopWindow()
        {
            if (IsTransitioning || _openWindows.Count == 0)
            {
                return;
            }

            LockTransitions();

            Screen window = _openWindows.Pop();
            window.Hide();
        }

        public void Back()
        {
            if (IsTransitioning)
            {
                return;
            }

            if (_openWindows.Count > 0)
            {
                CloseTopWindow();
            }
            else if (_currentScreen.TryGetComponent(out TavernScreen dungeonsScreen))
            {
                ShowScreen(ScreenType.MainMenu);
            }
            else if (_currentScreen.TryGetComponent(out GameScreen gameScreen))
            {
                var confirmData = new ConfirmData("Выход",
                    "Вы уверены, что хотите сбежать? Это приведёт к позору (потери прогресса боя)", onAccept: () =>
                    {
                        gameScreen.AbandonBattle();
                        ShowScreen(ScreenType.TavernScreen);
                    }, acceptText: "Сбежать", cancelText: "Остаться");

                SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.ConfirmWindow));
                SignalSystem.Raise<IConfirmHandler>(h => h.SetConfirmData(confirmData));
            }
            else if (_currentScreen.TryGetComponent(out InventoryScreen inventoryWindow))
            {
                ShowScreen(ScreenType.TavernScreen);
            }
            else if (_currentScreen.TryGetComponent(out MainMenuScreen mainMenuScreen))
            {
                var confirmData = new ConfirmData("Бежишь из таверны?",
                    "Трактирщик обидится, но переживёт. Выходим?", onAccept: QuitGame,
                    acceptText: "Выйти", cancelText: "Остаться");

                SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.ConfirmWindow));
                SignalSystem.Raise<IConfirmHandler>(h => h.SetConfirmData(confirmData));
            }
        }

        private static void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private Screen GetScreen(ScreenType screenType)
        {
            return screenType switch {
                ScreenType.MainMenu => _mainMenuScreen,
                ScreenType.GameScreen => _gameScreen,
                ScreenType.GameOverScreen => _gameOverScreen,
                ScreenType.LootScreen => _lootScreen,
                ScreenType.TavernScreen => _tavernScreen,

                ScreenType.OptionsWindow => _optionsWindow,
                ScreenType.InventoryWindow => _inventoryScreen,
                ScreenType.CreditsWindow => _creditsWindow,
                ScreenType.HelpWindow => _helpWindow,
                ScreenType.QuestWindow => _questWindow,
                ScreenType.ConfirmWindow => _confirmWindow,
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
            _inventoryScreen.gameObject.SetActive(false);
            _creditsWindow.gameObject.SetActive(false);
            _helpWindow.gameObject.SetActive(false);
            _questWindow.gameObject.SetActive(false);
            _confirmWindow.gameObject.SetActive(false);

            SignalSystem.Subscribe(this);
        }

        private void OnDestroy() => SignalSystem.Unsubscribe(this);
    }
}

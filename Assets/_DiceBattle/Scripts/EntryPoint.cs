using DiceBattle.Audio;
using DiceBattle.Screens;
using DiceBattle.UI;
using GameSignals;
using UnityEngine;

namespace DiceBattle
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private MainMenu _mainMenu;
        [SerializeField] private DungeonsScreen _dungeonsScreen;
        [SerializeField] private GameScreen _gameScreen;
        [SerializeField] private GameOverScreen _gameOverScreen;
        [SerializeField] private LootScreen _lootScreen;
        [Space]
        [SerializeField] private GameLoop _gameLoop;

        private void Start()
        {
            // _mainMenu.gameObject.SetActive(true);
            // _dungeonsScreen.gameObject.SetActive(false);
            // _gameOverScreen.gameObject.SetActive(false);
            // _gameScreen.gameObject.SetActive(false);
            // _lootScreen.gameObject.SetActive(false);

            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.MainMenu));

            _gameLoop.InitializeGame();
        }
    }
}

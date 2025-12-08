using DiceBattle.Screens;
using DiceBattle.UI;
using UnityEngine;

namespace DiceBattle
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private MainMenu _mainMenu;
        [SerializeField] private GameScreen _gameScreen;
        [SerializeField] private GameOverScreen _gameOverScreen;
        [SerializeField] private LootScreen _lootScreen;
        [Space]
        [SerializeField] private GameLoop _gameLoop;

        private void Start()
        {
            // TODO: SignalSystem.Raise - Background music

            _mainMenu.gameObject.SetActive(true);
            _gameOverScreen.gameObject.SetActive(false);
            _gameScreen.gameObject.SetActive(false);
            _lootScreen.gameObject.SetActive(false);

            _gameLoop.InitializeGame();
        }
    }
}

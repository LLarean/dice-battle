using DiceBattle.Screens;
using UnityEngine;

namespace DiceBattle
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private MainMenu _mainMenu;
        [SerializeField] private GameScreen _gameScreen;
        [SerializeField] private GameOverScreen _gameOverScreen;
        [SerializeField] private LootScreen _lootScreen;

        private void Start()
        {
            _mainMenu.gameObject.SetActive(true);
            _gameOverScreen.gameObject.SetActive(false);
            _gameScreen.gameObject.SetActive(false);
            _lootScreen.gameObject.SetActive(false);
        }
    }
}
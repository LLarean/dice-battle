using DiceBattle.Screens;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button _start;
        [SerializeField] private GameScreen _gameScreen;

        private void Start() => _start.onClick.AddListener(StartClick);

        private void OnDestroy() => _start.onClick.RemoveAllListeners();

        private void StartClick()
        {
            _gameScreen.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
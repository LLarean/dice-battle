using DiceBattle.Audio;
using DiceBattle.Screens;
using GameSignals;
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
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));

            _gameScreen.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}

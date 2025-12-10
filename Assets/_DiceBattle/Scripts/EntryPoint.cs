using DiceBattle.Audio;
using DiceBattle.UI;
using GameSignals;
using UnityEngine;

namespace DiceBattle
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameLoop _gameLoop;

        private void Start()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.MainMenu));

            _gameLoop.InitializeGame();
        }
    }
}

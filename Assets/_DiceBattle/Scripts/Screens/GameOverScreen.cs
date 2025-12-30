using System;
using DiceBattle.Events;
using GameSignals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.Screens
{
    public class GameOverScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _finalScore;
        [SerializeField] private Button _restart;

        public event Action OnRestartClicked;

        private void Start() => _restart.onClick.AddListener(HandleRestartClick);

        private void OnDestroy() => _restart.onClick.RemoveAllListeners();

        private void OnEnable()
        {
            _finalScore.text = $"Вы победили {GameProgress.CompletedLevels} врагов!"; // TODO Translation
        }

        private void HandleRestartClick()
        {
            GameProgress.ResetAll();
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.GameScreen));
        }
    }
}

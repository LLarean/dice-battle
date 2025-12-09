using System;
using DiceBattle.Audio;
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

        public void Show(int enemiesDefeated)
        {
            gameObject.SetActive(true);
            _finalScore.text = $"You have defeated {enemiesDefeated} enemies!";
        }

        private void Start() => _restart.onClick.AddListener(RestartClick);

        private void OnDestroy() => _restart.onClick.RemoveAllListeners();

        private void RestartClick()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
            OnRestartClicked?.Invoke();
        }
    }
}

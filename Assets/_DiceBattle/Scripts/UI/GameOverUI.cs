using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle
{
    public class GameOverUI : MonoBehaviour
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

        private void RestartClick() => OnRestartClicked?.Invoke();
    }
}
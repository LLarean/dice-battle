using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DiceBattle.Screens
{
    /// <summary>
    /// Control of the main UI of the game (Player's HP, buttons, Game Over)
    /// </summary>
    public class GameScreen : MonoBehaviour
    {
        [Header("Unit Panels")]
        [SerializeField] private UnitPanel _player;
        [SerializeField] private UnitPanel _enemy;
        
        [Header("Buttons")]
        [SerializeField] private Button _rollButton;
        [SerializeField] private Button _rerollButton;
        [SerializeField] private TextMeshProUGUI _rollButtonText;
        [Space]
        [SerializeField] private GameOverScreen _gameOverScreen;

        private int _playerMaxHP;

        /// <summary>
        /// Initializes the game UI with the player's maximum HP.
        /// </summary>
        /// <param name="maxHP">The maximum HP value for the player.</param>
        public void Initialize(int maxHP)
        {
            _player.HideAllStats();
            _player.SetHealthMax(maxHP);
        }

        /// <summary>
        /// Upgrade the player's HP
        /// </summary>
        public void UpdatePlayerHP(int currentHP)
        {
            _player.UpdateHealth(currentHP);
        }

        /// <summary>
        /// Show the player's current defense (reset to zero each turn)
        /// </summary>
        public void UpdatePlayerDefense(int defense)
        {
            _player.UpdateDefense(defense);
        }

        /// <summary>
        /// Set the status of the "Quit" button/Redefine"
        /// </summary>
        public void SetRollButtonState(bool isFirstRoll, bool interactable)
        {
            if (_rollButton != null)
                _rollButton.interactable = interactable;

            if (_rollButtonText != null)
                _rollButtonText.text = isFirstRoll ? "Roll the dice" : "Complete the turn";
        }

        /// <summary>
        /// Set the status of the "Redo" button
        /// </summary>
        public void SetRerollButtonState(bool visible, bool interactable)
        {
            if (_rerollButton != null)
            {
                _rerollButton.gameObject.SetActive(visible);
                _rerollButton.interactable = interactable;
            }
        }

        /// <summary>
        /// Show the Game Over screen
        /// </summary>
        public void ShowGameOver(int enemiesDefeated) => _gameOverScreen.Show(enemiesDefeated);

        /// <summary>
        /// Hide the Game Over screen
        /// </summary>
        public void HideGameOver() => _gameOverScreen.gameObject.SetActive(false);

        /// <summary>
        /// Subscribe to button events (called from GameManager)
        /// </summary>
        public void SubscribeToButtons(Action onRoll, Action onReroll)
        {
            _rollButton.onClick.AddListener(() => onRoll?.Invoke());
            _rerollButton.onClick.AddListener(() => onReroll?.Invoke());
        }
    }
}
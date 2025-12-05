using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DiceBattle.Core;

namespace DiceBattle.UI
{
    /// <summary>
    /// Displaying information about the enemy
    /// </summary>
    public class EnemyUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _enemyNumberText;
        [SerializeField] private TextMeshProUGUI _hpText;
        [SerializeField] private TextMeshProUGUI _attackText;
        [SerializeField] private TextMeshProUGUI _defenseText;
        [SerializeField] private Image _portrait;
        [SerializeField] private Slider _hpSlider; // Optional - HP strip

        private Enemy _currentEnemy;

        /// <summary>
        /// Show a new enemy
        /// </summary>
        public void ShowEnemy(Enemy enemy)
        {
            _currentEnemy = enemy;
            UpdateDisplay();
        }

        /// <summary>
        /// Update the display (HP has changed)
        /// </summary>
        public void UpdateDisplay()
        {
            if (_currentEnemy == null)
                return;

            // The enemy's number
            if (_enemyNumberText != null)
                _enemyNumberText.text = $"Enemy #{_currentEnemy.Number + 1}";

            // HP
            if (_hpText != null)
                _hpText.text = $"HP: {_currentEnemy.CurrentHP}/{_currentEnemy.MaxHP}";

            // HP Slider
            if (_hpSlider != null)
            {
                _hpSlider.maxValue = _currentEnemy.MaxHP;
                _hpSlider.value = _currentEnemy.CurrentHP;
            }

            // Attack
            if (_attackText != null)
                _attackText.text = $"Attack: {_currentEnemy.Attack}";

            // Defense
            if (_defenseText != null)
                _defenseText.text = $"Defense: {_currentEnemy.Defense}";

            // Portrait
            if (_portrait != null)
                _portrait.sprite = _currentEnemy.Portrait;
        }

        /// <summary>
        /// Damage animation (optional)
        /// </summary>
        public void PlayHitAnimation()
        {
            // TODO: Add a shake or blink animation
        }
    }
}

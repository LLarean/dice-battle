using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DiceBattle.UI
{
    public class EnemyUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _enemyNumberText;
        [SerializeField] private TextMeshProUGUI _hpText;
        [SerializeField] private TextMeshProUGUI _attackText;
        [SerializeField] private TextMeshProUGUI _defenseText;
        [SerializeField] private Image _portrait;
        [SerializeField] private Slider _hpSlider; // Optional - HP strip

        private UnitData _unitData;

        public void ShowEnemy(UnitData unitData)
        {
            _unitData = unitData;
            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            _enemyNumberText.text = _unitData.Title;
            _portrait.sprite = _unitData.Portrait;

            _hpText.text = $"HP: {_unitData.CurrentHealth}/{_unitData.MaxHealth}";
            _hpSlider.maxValue = _unitData.MaxHealth;
            _hpSlider.value = _unitData.CurrentHealth;

            _attackText.text = $"Attack: {_unitData.Attack}";
            _defenseText.text = $"Defense: {_unitData.Defense}";
        }

        public void PlayHitAnimation()
        {
            // TODO: Add a shake or blink animation
        }
    }
}

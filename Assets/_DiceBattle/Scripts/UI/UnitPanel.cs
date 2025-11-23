using DiceBattle.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle
{
    public class UnitPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Image _portrait;
        [SerializeField] private Slider _health;
        [SerializeField] private UnitStats _unitStats;
        
        private Enemy _currentEnemy;

        public void Construct(UnitData unitData)
        {
            _title.text = unitData.Title;
            _portrait.sprite = unitData.Portrait;

            _health.maxValue = unitData.HealthMax;
            _health.value = unitData.HealthCurrent;

            _unitStats.HideAll();
            
            _unitStats.ShowHealth($"{unitData.HealthCurrent}/{unitData.HealthMax}");

            if (unitData.Attack != 0)
            {
                _unitStats.ShowAttack(unitData.Attack.ToString());
            }
            
            if (unitData.Defence != 0)
            {
                _unitStats.ShowDefense(unitData.Defence.ToString());
            }
        }

        public void SetHealthMax(int maxHealth)
        {
            _health.maxValue = maxHealth;
            _unitStats.ShowHealth($"{maxHealth}/{maxHealth}");
        }

        public void UpdateHealth(int currentHealth)
        {
            _health.value = currentHealth;
            _unitStats.ShowHealth($"{currentHealth}/{_health.maxValue}");
        }

        public void UpdateDefense(int defense)
        {
        }

        public void ShowEnemy(Enemy currentEnemy)
        {
            _currentEnemy = currentEnemy;
            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            _title.text = $"Enemy #{_currentEnemy.Number}";
            _portrait.sprite = _currentEnemy.Portrait;

            SetHealthMax(_currentEnemy.MaxHP);
            UpdateHealth(_currentEnemy.CurrentHP);

            _unitStats.ShowAttack(_currentEnemy.Attack.ToString());
            _unitStats.ShowDefense(_currentEnemy.Defense.ToString());
        }
    }
}

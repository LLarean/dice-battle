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

        public void HideAllStats()
        {
            _unitStats.HideHealth();
            _unitStats.HideAttack();
            _unitStats.HideDefense();
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
        
        public void UpdateAttack(int attack)
        {
            if (attack > 0)
            {
                _unitStats.ShowAttack(attack.ToString());
            }
            else
            {
                _unitStats.HideAttack();
            }
        }

        public void UpdateDefense(int defense)
        {
            if (defense > 0)
            {
                _unitStats.ShowDefense(defense.ToString());
            }
            else
            {
                _unitStats.HideDefense();
            }
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

            UpdateAttack(_currentEnemy.Attack);
            UpdateDefense(_currentEnemy.Defense);
        }
    }
}

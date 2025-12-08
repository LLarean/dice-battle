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

        private UnitData _unitData;

        public void SetUnitData(UnitData unitData)
        {
            _unitData = unitData;
            HideAllStats();
            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            _title.text = _unitData.Title;
            _portrait.sprite = _unitData.Portrait;

            SetMaxHealth(_unitData.HealthMax);
            UpdateCurrentHealth(_unitData.HealthCurrent);

            UpdateAttack(_unitData.Attack);
            UpdateDefense(_unitData.Defense);
        }

        public void HideAllStats()
        {
            _unitStats.HideHealth();
            _unitStats.HideAttack();
            _unitStats.HideDefense();
        }

        public void SetMaxHealth(int maxHealth)
        {
            _health.maxValue = maxHealth;
            _health.value = _health.maxValue;
            _unitStats.ShowHealth($"{maxHealth}/{maxHealth}");
        }

        public void UpdateCurrentHealth(int currentHealth)
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
    }
}

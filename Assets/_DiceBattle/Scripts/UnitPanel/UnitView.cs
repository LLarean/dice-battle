using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle
{
    public class UnitView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Image _portrait;
        [SerializeField] private Slider _health;
        [SerializeField] private UnitStats _unitStats;

        public void Construct(in UnitModel unitModel)
        {
            ResetUnitStats();
            
            _title.text = unitModel.Title;
            _portrait.sprite = unitModel.Portrait;
            
            UpdateStats(unitModel);
        }

        public void UpdateCurrentHealth(int currentHealth)
        {
            _health.value = currentHealth;
            _unitStats.ShowHealth($"{currentHealth}/{_health.maxValue}");
        }

        private void ResetUnitStats()
        {
            _unitStats.HideHealth();
            _unitStats.HideAttack();
            _unitStats.HideDefense();
        }

        private void UpdateStats(UnitModel unitModel)
        {
            UpdateMaxHealth(unitModel);

            if (unitModel.Attack != 0)
            {
                _unitStats.ShowAttack(unitModel.Attack.ToString());
            }
            
            if (unitModel.Defence != 0)
            {
                _unitStats.ShowDefense(unitModel.Defence.ToString());
            }
        }

        private void UpdateMaxHealth(UnitModel unitModel)
        {
            _health.maxValue = unitModel.MaxHealth;
            _health.value = unitModel.MaxHealth;
            UpdateCurrentHealth(unitModel.MaxHealth);
        }
    }
}
using DiceBattle.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class UnitPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Image _portrait;
        [SerializeField] private Slider _health;
        [SerializeField] private UnitStats _stats;

        private UnitData _unitData;

        public void SetUnitData(UnitData unitData)
        {
            _unitData = unitData;
            _title.text = _unitData.Title;
            _portrait.sprite = _unitData.Portrait;

            UpdateStats();
        }

        public void UpdateStats()
        {
            HideAllStats();

            SetMaxHealth(_unitData.MaxHealth);
            SetCurrentHealth(_unitData.CurrentHealth);
            SetAttack(_unitData.Damage);
            SetArmor(_unitData.Armor);
        }

        public void TakeDamage(int damageAmount)
        {
            int calculatedDamage = Mathf.Max(0, damageAmount - _unitData.Armor);
            _unitData.CurrentHealth = Mathf.Max(0, _unitData.CurrentHealth - calculatedDamage);
            _health.value = _unitData.CurrentHealth;
            _stats.ShowHealth($"{_unitData.CurrentHealth}/{_health.maxValue}");
        }

        public void TakeHeal(int healAmount)
        {
            _unitData.CurrentHealth = Mathf.Min(_unitData.MaxHealth, _unitData.CurrentHealth + healAmount);
            _health.value = _unitData.CurrentHealth;
            _stats.ShowHealth($"{_unitData.CurrentHealth}/{_health.maxValue}");
        }

        public void AnimateHeal() => HealthAnimation.AnimateHeal(_portrait);

        public void AnimateDamage() => HealthAnimation.AnimateDamage(_portrait);

        private void SetMaxHealth(int healthAmount)
        {
            _health.maxValue = healthAmount;
        }

        private void SetCurrentHealth(int healthAmount)
        {
            _health.value = healthAmount;
            _stats.ShowHealth($"{_health.value}/{_health.maxValue}");
        }

        private void SetAttack(int attackAmount)
        {
            if (attackAmount > 0)
            {
                _stats.ShowAttack(attackAmount.ToString());
            }
            else
            {
                _stats.HideAttack();
            }
        }

        private void SetArmor(int defense)
        {
            if (defense > 0)
            {
                _stats.ShowArmor(defense.ToString());
            }
            else
            {
                _stats.HideArmor();
            }
        }

        private void HideAllStats()
        {
            _stats.HideHealth();
            _stats.HideAttack();
            _stats.HideArmor();
        }
    }
}

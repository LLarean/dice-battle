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
        private int _armorBonus;
        private int _damageBonus;
        private int _healBonus;

        public void SetUnitData(UnitData unitData)
        {
            _unitData = unitData;
            _title.text = _unitData.Name;
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

        public void SetDicePreview(int armorBonus, int damageBonus, int healBonus)
        {
            _armorBonus = armorBonus;
            _damageBonus = damageBonus;
            _healBonus = healBonus;

            SetCurrentHealth(_unitData.CurrentHealth);
            SetAttack(_unitData.Damage);
            SetArmor(_unitData.Armor);
        }

        public void ClearDicePreview() => SetDicePreview(0, 0, 0);

        public void TakeDamage(int damageAmount)
        {
            int calculatedDamage = Mathf.Max(0, damageAmount - _unitData.Armor);
            _unitData.CurrentHealth = Mathf.Max(0, _unitData.CurrentHealth - calculatedDamage);
            _health.value = _unitData.CurrentHealth;
            SetCurrentHealth(_unitData.CurrentHealth);
        }

        public void TakeHeal(int healAmount)
        {
            _unitData.CurrentHealth = Mathf.Min(_unitData.MaxHealth, _unitData.CurrentHealth + healAmount);
            _health.value = _unitData.CurrentHealth;
            SetCurrentHealth(_unitData.CurrentHealth);
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

            int previewHealth = Mathf.Min(_unitData.MaxHealth, healthAmount + _healBonus);
            int healthBonus = previewHealth - healthAmount;
            _stats.ShowHealth($"{healthAmount}/{_health.maxValue}", healthBonus);
        }

        private void SetAttack(int attackAmount)
        {
            if (attackAmount > 0 || _damageBonus > 0)
            {
                _stats.ShowAttack(attackAmount.ToString(), _damageBonus);
            }
            else
            {
                _stats.HideAttack();
            }
        }

        private void SetArmor(int defense)
        {
            if (defense > 0 || _armorBonus > 0)
            {
                _stats.ShowArmor(defense.ToString(), _armorBonus);
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

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
        [SerializeField] private UnitStats _unitStats;

        private HealthAnimation _healthAnimation;
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

            SetMaxHealth(_unitData.MaxHealth);
            UpdateCurrentHealth(_unitData.CurrentHealth);

            UpdateAttack(_unitData.Damage);
            UpdateArmor(_unitData.Armor);
        }

        public void TakeDamage(int damageAmount)
        {
            int currentDamage = Mathf.Max(0, damageAmount - _unitData.Armor);
            _unitData.CurrentHealth = Mathf.Max(0, _unitData.CurrentHealth - currentDamage);
            _health.value = _unitData.CurrentHealth;
            _unitStats.ShowHealth($"{_unitData.CurrentHealth}/{_health.maxValue}");
        }

        public void TakeHeal(int healAmount)
        {
            _unitData.CurrentHealth = Mathf.Min(_unitData.MaxHealth, _unitData.CurrentHealth + healAmount);
            _health.value = _unitData.CurrentHealth;
            _unitStats.ShowHealth($"{_unitData.CurrentHealth}/{_health.maxValue}");
        }

        // public void AnimationCurrentHealth(int currentHealth) => _healthAnimation.Animate(currentHealth);

        public void AnimateHeal() => _healthAnimation.AnimateHeal();

        public void AnimateDamage() => _healthAnimation.AnimateDamage();

        public void UpdateArmor(int defense)
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

        private void SetMaxHealth(int maxHealth)
        {
            _health.maxValue = maxHealth;
            _health.value = _health.maxValue;
            _unitStats.ShowHealth($"{maxHealth}/{maxHealth}");
        }

        private void UpdateCurrentHealth(int currentHealth)
        {
            // AnimateHealth(currentHealth);
            _health.value = currentHealth;
            _unitStats.ShowHealth($"{currentHealth}/{_health.maxValue}");
            // TODO LLarean Separate the logic of taking damage and healing so that you can animate
        }

        private void UpdateAttack(int attack)
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

        private void HideAllStats()
        {
            _unitStats.HideHealth();
            _unitStats.HideAttack();
            _unitStats.HideDefense();
        }

        private void Awake()
        {
            _healthAnimation = new HealthAnimation(_portrait);
        }
    }
}

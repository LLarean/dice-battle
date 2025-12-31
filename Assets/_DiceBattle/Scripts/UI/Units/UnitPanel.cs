using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class UnitPanel : MonoBehaviour
    {
        private const float _flashDuration = 0.1f;
        private const int _flashCount = 3;

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

            SetMaxHealth(_unitData.MaxHealth);
            UpdateCurrentHealth(_unitData.CurrentHealth);

            UpdateAttack(_unitData.Attack);
            UpdateArmor(_unitData.Armor);
        }

        public void UpdateCurrentHealth(int currentHealth)
        {
            AnimateHealth(currentHealth);
            _health.value = currentHealth;
            _unitStats.ShowHealth($"{currentHealth}/{_health.maxValue}");
            // TODO LLarean Separate the logic of taking damage and healing so that you can animate
        }

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

        private void AnimateHealth(int currentHealth)
        {
            Color flashColor = currentHealth > _health.value ? Color.green : Color.red;

            LeanTween.cancel(_portrait.gameObject);

            LTSeq sequence = LeanTween.sequence();

            for (int i = 0; i < _flashCount; i++)
            {
                sequence.append(LeanTween.value(_portrait.gameObject, _portrait.color, flashColor, _flashDuration)
                    .setOnUpdate(val => _portrait.color = val));

                sequence.append(LeanTween.value(_portrait.gameObject, flashColor, Color.white, _flashDuration)
                    .setOnUpdate(val => _portrait.color = val));
            }

            sequence.append(() => _portrait.color = Color.white);
        }
    }
}

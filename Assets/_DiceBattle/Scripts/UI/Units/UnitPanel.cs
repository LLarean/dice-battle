using System.Collections.Generic;
using DiceBattle.Animations;
using DiceBattle.Core;
using DiceBattle.Events;
using GameSignals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class UnitPanel : MonoBehaviour, IDiceResultHandler
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Image _portrait;
        [SerializeField] private Slider _health;
        [SerializeField] private UnitStats _stats;
        [Tooltip("Источник кубиков, чьи результаты отображает эта панель. Оставить пустым, если у юнита нет собственных кубиков.")]
        [SerializeField] private DiceHolder _diceHolder;

        private readonly Dictionary<Dice, int> _armorByDice = new();
        private readonly Dictionary<Dice, int> _damageByDice = new();
        private readonly Dictionary<Dice, int> _healByDice = new();

        private UnitData _unitData;
        private int _armorBonus;
        private int _damageBonus;
        private int _healBonus;
        private int _equipmentArmorBonus;
        private int _equipmentDamageBonus;

        public void SetUnitData(UnitData unitData)
        {
            _unitData = unitData;
            _title.text = _unitData.Name;
            _portrait.sprite = _unitData.Portrait;
            _equipmentArmorBonus = 0;
            _equipmentDamageBonus = 0;

            UpdateStats();
        }

        public void UpdateStats()
        {
            SetMaxHealth(_unitData.MaxHealth);
            SetCurrentHealth(_unitData.CurrentHealth);
            SetAttack(_unitData.Damage);
            SetArmor(_unitData.Armor);
        }

        public void SetEquipmentBonus(int? armorBonus, int? damageBonus)
        {
            if (armorBonus.HasValue)
            {
                _equipmentArmorBonus = armorBonus.Value;
                SetArmor(_unitData.Armor);
            }

            if (damageBonus.HasValue)
            {
                _equipmentDamageBonus = damageBonus.Value;
                SetAttack(_unitData.Damage);
            }
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

        public void ClearDicePreview()
        {
            _armorByDice.Clear();
            _damageByDice.Clear();
            _healByDice.Clear();

            SetDicePreview(0, 0, 0);
        }

        public void OnDiceLanded(DiceHolder source, Dice dice, int amount)
        {
            if (source != _diceHolder)
            {
                return;
            }

            switch (dice.DiceValue)
            {
                case DiceValue.Attack:
                    _damageByDice[dice] = amount;
                    break;
                case DiceValue.Defense:
                    _armorByDice[dice] = amount;
                    break;
                case DiceValue.Heal:
                    _healByDice[dice] = amount;
                    break;
                default:
                    return;
            }

            SetDicePreview(Sum(_armorByDice), Sum(_damageByDice), Sum(_healByDice));
        }

        private static int Sum(Dictionary<Dice, int> bonusByDice)
        {
            int sum = 0;

            foreach (int value in bonusByDice.Values)
            {
                sum += value;
            }

            return sum;
        }

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
            _stats.SetHealth($"{healthAmount}/{_health.maxValue}", healthBonus);
        }

        private void SetAttack(int attackAmount) =>
            _stats.SetAttack((attackAmount - _equipmentDamageBonus).ToString(), _equipmentDamageBonus + _damageBonus);

        private void SetArmor(int defense) =>
            _stats.SetArmor((defense - _equipmentArmorBonus).ToString(), _equipmentArmorBonus + _armorBonus);

        private void Awake() => SignalSystem.Subscribe(this);

        private void OnDestroy() => SignalSystem.Unsubscribe(this);
    }
}

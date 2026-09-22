using System;
using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts;
using DiceBattle.Animations;
using DiceBattle.Core;
using DiceBattle.Events;
using DiceBattle.Localization;
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
        [Tooltip("The source of the dice whose results are displayed by this panel. Leave empty if the unit has no dice of its own.")]
        [SerializeField] private DiceHolder _diceHolder;

        private readonly Dictionary<Dice, int> _armorByDice = new();
        private readonly Dictionary<Dice, int> _damageByDice = new();
        private readonly Dictionary<Dice, int> _healByDice = new();
        private readonly List<GameObject> _flyingNumbers = new();

        private UnitData _unitData;
        private (int Armor, int Damage, int Heal) _pendingPreview;
        private int _armorBonus;
        private int _damageBonus;
        private int _healBonus;
        private int _equipmentArmorBonus;
        private int _equipmentDamageBonus;

        public void SetUnitData(UnitData unitData)
        {
            _unitData = unitData;
            _title.text = LocalizationManager.HasKey(_unitData.Name)
                ? LocalizationManager.Localize(_unitData.Name)
                : _unitData.Name;
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
            _pendingPreview = (armorBonus, damageBonus, healBonus);

            // Numbers still in flight will apply the preview on arrival.
            if (_flyingNumbers.Count == 0)
            {
                ShowDicePreview(_pendingPreview);
            }
        }

        private void ShowDicePreview((int Armor, int Damage, int Heal) preview)
        {
            _armorBonus = preview.Armor;
            _damageBonus = preview.Damage;
            _healBonus = preview.Heal;

            SetCurrentHealth(_unitData.CurrentHealth);
            SetAttack(_unitData.Damage);
            SetArmor(_unitData.Armor);
        }

        public void ClearDicePreview()
        {
            CancelFlyingNumbers();
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

            StatItem target;

            switch (dice.DiceValue)
            {
                case DiceValue.Attack:
                    _damageByDice[dice] = amount;
                    target = _stats.Attack;
                    break;
                case DiceValue.Defense:
                    _armorByDice[dice] = amount;
                    target = _stats.Armor;
                    break;
                case DiceValue.Heal:
                    _healByDice[dice] = amount;
                    target = _stats.Health;
                    break;
                default:
                    return;
            }

            _pendingPreview = (Sum(_armorByDice), Sum(_damageByDice), Sum(_healByDice));
            FlyNumberToStat(dice, amount, target, _pendingPreview);
        }

        private void FlyNumberToStat(Dice dice, int amount, StatItem target, (int, int, int) previewOnArrival)
        {
            TMP_Text label = FloatingText.Spawn(target.Label, dice.transform.position, $"+{amount}", FloatingText.Positive);
            GameObject number = label.gameObject;
            _flyingNumbers.Add(number);

            FloatingTextAnimation.FlyTo(label, target.Label.transform, () =>
            {
                _flyingNumbers.Remove(number);
                ShowDicePreview(_flyingNumbers.Count == 0 ? _pendingPreview : previewOnArrival);
            });
        }

        private void CancelFlyingNumbers()
        {
            foreach (GameObject number in _flyingNumbers)
            {
                LeanTween.cancel(number);
                Destroy(number);
            }

            _flyingNumbers.Clear();
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
            ChangeHealth(Mathf.Max(0, _unitData.CurrentHealth - calculatedDamage));
        }

        public void TakeHeal(int healAmount)
        {
            ChangeHealth(Mathf.Min(_unitData.MaxHealth, _unitData.CurrentHealth + healAmount));
        }

        public void TakeCriticalHit()
        {
            _unitData.CurrentHealth = 0;
            _health.value = 0;
            SetCurrentHealth(0);

            string text = LocalizationManager.Localize(LocKeys.GameHits.Critical);
            TMP_Text label = FloatingText.Spawn(_title, _portrait.transform.position, text, FloatingText.Critical, 2f);
            FloatingTextAnimation.Rise(label, 1.6f);
        }

        private void ChangeHealth(int newHealth)
        {
            int delta = newHealth - _unitData.CurrentHealth;

            _unitData.CurrentHealth = newHealth;
            _health.value = newHealth;
            SetCurrentHealth(newHealth);

            if (delta != 0)
            {
                ShowHealthDelta(delta);
            }
        }

        private void ShowHealthDelta(int delta)
        {
            string text = delta > 0 ? $"+{delta}" : delta.ToString();
            Color color = delta > 0 ? FloatingText.Positive : FloatingText.Negative;

            TMP_Text label = FloatingText.Spawn(_title, _portrait.transform.position, text, color, 1.5f);
            FloatingTextAnimation.Rise(label);
        }

        public void AnimateCharacterSwap(int direction, Action onSwap)
        {
            bool swapped = false;

            void SwapOnce()
            {
                if (swapped)
                {
                    return;
                }

                swapped = true;
                onSwap?.Invoke();
            }

            PortraitSwapAnimation.AnimateSwap(_portrait.rectTransform, direction, SwapOnce);
            TextSwapAnimation.AnimateSwap(_title, SwapOnce);
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

        private void OnDisable() => CancelFlyingNumbers();

        private void OnDestroy() => SignalSystem.Unsubscribe(this);
    }
}

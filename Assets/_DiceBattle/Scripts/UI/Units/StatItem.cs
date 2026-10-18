using DiceBattle.Animations;
using TMPro;
using UnityEngine;

namespace DiceBattle.UI
{
    public class StatItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _value;

        private const string BonusColor = "#4CD137";
        private const string LossColor = "#E8401A";
        private const string CriticalLossColor = "#FFC71A";

        private string _lastText;

        public TMP_Text Label => _value;

        public void SetValue(string value) => SetValue(value, 0);

        public void SetValue(string value, int bonus, int loss = 0, bool isCriticalLoss = false)
        {
            string text = bonus > 0 ? $"{value} <color={BonusColor}>+{bonus}</color>" : value;

            if (loss > 0)
            {
                text += $" <color={(isCriticalLoss ? CriticalLossColor : LossColor)}>-{loss}</color>";
            }

            if (_lastText != null && text != _lastText)
            {
                StatValueAnimation.AnimatePunch(_value.transform);
            }

            _lastText = text;
            _value.text = text;
        }
    }
}

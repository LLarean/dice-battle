using DiceBattle.Animations;
using TMPro;
using UnityEngine;

namespace DiceBattle.UI
{
    public class StatItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _value;

        private const string BonusColor = "#4CD137";

        private string _lastText;

        public void SetValue(string value) => SetValue(value, 0);

        public void SetValue(string value, int bonus)
        {
            string text = bonus > 0 ? $"{value} <color={BonusColor}>+{bonus}</color>" : value;

            if (_lastText != null && text != _lastText)
            {
                StatValueAnimation.AnimatePunch(_value.transform);
            }

            _lastText = text;
            _value.text = text;
        }
    }
}

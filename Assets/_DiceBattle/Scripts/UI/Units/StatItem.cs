using TMPro;
using UnityEngine;

namespace DiceBattle.UI
{
    public class StatItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _value;

        private const string BonusColor = "#4CD137";

        public void SetValue(string value) => _value.text = value;

        public void SetValue(string value, int bonus)
        {
            _value.text = bonus > 0 ? $"{value} <color={BonusColor}>+{bonus}</color>" : value;
        }
    }
}

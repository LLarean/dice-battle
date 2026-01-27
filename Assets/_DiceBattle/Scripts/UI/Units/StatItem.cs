using TMPro;
using UnityEngine;

namespace DiceBattle.UI
{
    public class StatItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _value;

        public void SetValue(string value) => _value.text = value;
    }
}

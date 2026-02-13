using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class RewardItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _title;

        private DiceType _diceType;

        public event Action<DiceType> OnClicked;

        public void SetReward(DiceType diceType)
        {
            _diceType = diceType;
            _title.text = diceType.Description();
        }

        private void Start() => _button.onClick.AddListener(HandleClick);

        private void OnDestroy() => _button.onClick.RemoveAllListeners();

        private void HandleClick() => OnClicked?.Invoke(_diceType);
    }
}

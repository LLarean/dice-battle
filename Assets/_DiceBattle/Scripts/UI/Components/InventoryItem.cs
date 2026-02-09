using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class InventoryItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _agreeMark;
        [SerializeField] private TextMeshProUGUI _title;

        private RewardType _rewardType;

        public RewardType RewardType => _rewardType;

        public event Action<RewardType> OnClicked;

        public void Construct(RewardType rewardType)
        {
            _rewardType = rewardType;
            _title.text = rewardType.Title();
        }

        private void Start() => _button.onClick.AddListener(HandleButtonClicked);

        private void OnDestroy() => _button.onClick.RemoveAllListeners();

        private void HandleButtonClicked()
        {
            OnClicked?.Invoke(_rewardType);
            _agreeMark.gameObject.SetActive(!_agreeMark.gameObject.activeSelf);
        }
    }
}

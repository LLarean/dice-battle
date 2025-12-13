using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle
{
    public class RewardItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _title;

        public event Action OnClicked;

        public void SetReward(RewardType rewardType) => _title.text = rewardType.ToString();

        private void Start() => _button.onClick.AddListener(HandleClick);

        private void OnDestroy() => _button.onClick.RemoveAllListeners();

        private void HandleClick() => OnClicked?.Invoke();
    }
}

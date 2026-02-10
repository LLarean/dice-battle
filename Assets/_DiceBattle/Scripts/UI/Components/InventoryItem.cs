using System;
using DiceBattle.Audio;
using DiceBattle.Events;
using GameSignals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    [RequireComponent(typeof(Button))]
    public class InventoryItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [Space]
        [SerializeField] private Image _agreeMark;
        [SerializeField] private TextMeshProUGUI _title;

        private RewardType _rewardType;

        public RewardType RewardType => _rewardType;

        public event Action<RewardType> OnClicked;

        public void Initialize(RewardType rewardType)
        {
            _rewardType = rewardType;
            _title.text = rewardType.Title();
        }

        public void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
        }

        public void SetAgreeMark(bool isAgree)
        {
            _agreeMark.gameObject.SetActive(isAgree);
        }

        private void Start()
        {
            _button.onClick.AddListener(HandleButtonClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void HandleButtonClicked()
        {
            OnClicked?.Invoke(_rewardType);
            SetAgreeMark(!_agreeMark.gameObject.activeSelf);
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
        }
    }
}

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
        [SerializeField] private TextMeshProUGUI _description;

        private DiceType _diceType;

        public DiceType DiceType => _diceType;

        public event Action<DiceType> OnClicked;

        public void Initialize(DiceType diceType)
        {
            _diceType = diceType;
            _title.text = diceType.Title();
            _description.text = diceType.Title();
        }

        public void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
        }

        public void SetAgreeMark(bool isAgree)
        {
            // _agreeMark.gameObject.SetActive(isAgree);
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
            OnClicked?.Invoke(_diceType);
            SetAgreeMark(!_agreeMark.gameObject.activeSelf);
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
        }
    }
}

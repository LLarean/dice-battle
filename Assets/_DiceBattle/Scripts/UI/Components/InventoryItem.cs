using System;
using DiceBattle.Audio;
using DiceBattle.Core;
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
        [SerializeField] private Dice _dice;
        [SerializeField] private Image _agreeMark;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;

        private DiceType _diceType;

        public DiceType DiceType => _diceType;

        public event Action<DiceType> OnDiceToggled;

        public void Initialize(DiceType diceType)
        {
            _diceType = diceType;
            _title.text = diceType.Title();
            _description.text = diceType.Title();

            _dice.OnToggled += HandleDiceToggled;
        }

        public void SetEquippedStatus(bool isEquipped)
        {
            _agreeMark.gameObject.SetActive(isEquipped);
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
            OnDiceToggled?.Invoke(_diceType);
            SetEquippedStatus(!_agreeMark.gameObject.activeSelf);
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
        }

        private void HandleDiceToggled()
        {
            OnDiceToggled?.Invoke(_diceType);
        }
    }
}

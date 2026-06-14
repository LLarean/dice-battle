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

        private Item _data;
        private Transform _diceSlot;
        private bool _diceExtracted;

        public event Action<DiceType> OnDiceToggled;

        public Item Data => _data;
        public Dice Dice => _dice;

        public void Initialize(Item item)
        {
            _data = item;
            _title.text = item.Type.Title();
            _description.text = item.Type.Description();
        }

        public void SetEquippedStatus(bool isEquipped)
        {
            _dice.SetSelection(isEquipped);
            _agreeMark.gameObject.SetActive(isEquipped);
        }

        public Dice ExtractDice()
        {
            _diceExtracted = true;
            _agreeMark.gameObject.SetActive(true);
            return _dice;
        }

        public void ReturnDice()
        {
            _diceExtracted = false;
            _dice.transform.SetParent(_diceSlot);
            _dice.transform.localPosition = Vector3.zero;
            _agreeMark.gameObject.SetActive(false);
        }

        private void Awake()
        {
            _diceSlot = _dice.transform.parent;
        }

        private void Start()
        {
            _button.onClick.AddListener(HandleButtonClicked);
            _dice.OnToggled += HandleDiceClicked;
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
            _dice.OnToggled -= HandleDiceClicked;
        }

        private void HandleButtonClicked()
        {
            OnDiceToggled?.Invoke(_data.Type);
        }

        private void HandleDiceClicked()
        {
            if (_diceExtracted)
            {
                return;
            }

            OnDiceToggled?.Invoke(_data.Type);
        }
    }
}

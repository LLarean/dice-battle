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

        public event Action<DiceType> OnDiceToggled;

        public Item Data => _data;
        public Dice Dice => _dice;

        public void Initialize(Item item)
        {
            _data = item;
            _title.text = item.Type.Title();
            _description.text = item.Type.Description();
            _dice.SetFixedFace(item.Type.GetIconCategory());
        }

        public void SetEquippedStatus(bool isEquipped)
        {
            _agreeMark.gameObject.SetActive(isEquipped);
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
            OnDiceToggled?.Invoke(_data.Type);
        }
    }
}

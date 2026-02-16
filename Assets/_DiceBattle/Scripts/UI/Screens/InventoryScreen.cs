using System;
using System.Collections.Generic;
using DiceBattle.Core;
using DiceBattle.Global;
using UnityEngine;
using System.Linq;
using DiceBattle.Data;
using TMPro;

namespace DiceBattle.UI
{
    public class InventoryScreen : Screen
    {
        private readonly List<Dice> _dices = new();
        private readonly List<InventoryItem> _items = new();

        [Space]
        [SerializeField] private GameConfig _gameConfig;
        [Space]
        [SerializeField] private UnitPanel _unitPanel;
        [SerializeField] private DiceHolder _diceHolder;
        [Space]
        [SerializeField] private Dice _dice;
        [SerializeField] private Transform _diceSpawn;
        [Space]
        [SerializeField] private TextMeshProUGUI _itemsPlaceholder;
        [SerializeField] private InventoryItem _item;
        [SerializeField] private Transform _itemsSpawn;

        private void Start()
        {
            RefreshInventoryDisplay();
            GenerateDice();
            SetUnitData();

            _itemsPlaceholder.gameObject.SetActive(_items.Count == 0);
            _diceHolder.Initialize(_dices);
            _diceHolder.RepositionDice();
        }

        private void OnEnable()
        {
            RefreshInventoryDisplay();
        }

        private void RefreshInventoryDisplay()
        {
            GenerateItems();
            ToggleItems();
        }

        private void GenerateItems()
        {
            ClearItems();
            DiceList playerInventory = GameData.GetInventory();

            foreach (DiceType diceType in playerInventory.DiceTypes)
            {
                InventoryItem inventoryItem = Instantiate(_item, _itemsSpawn);
                inventoryItem.Initialize(diceType);
                _items.Add(inventoryItem);
            }
        }

        private void ClearItems()
        {
            foreach (InventoryItem inventoryItem in _items)
            {
                Destroy(inventoryItem.gameObject);
            }

            _items.Clear();
        }

        private void ToggleItems()
        {
            DiceList equippedRewards = GameData.GetEquippedItems();

            foreach (InventoryItem item in _items)
            {
                bool isReceived = equippedRewards.DiceTypes.Contains(item.DiceType);
                item.SetAgreeMark(isReceived);
            }
        }

        private void GenerateDice()
        {
            ClearDice();

            for (int i = 0; i < 5; i++)
            {
                Dice dice = Instantiate(_dice, _diceSpawn);
                dice.DisableButton();
                _dices.Add(dice);
            }
        }

        private void ClearDice()
        {
            foreach (Dice dice in _dices)
            {
                Destroy(dice.gameObject);
            }

            _dices.Clear();
        }

        private void SetUnitData()
        {
            var unitData = new UnitData
            {
                Title = "Heroes (you)",
                Portrait = _gameConfig.Player.Portraits[0],
                MaxHealth = _gameConfig.Player.StartHealth,
                CurrentHealth = _gameConfig.Player.StartHealth,
                Damage = _gameConfig.Player.StartDamage,
                Armor = _gameConfig.Player.StartArmor
            };

            _unitPanel.SetUnitData(unitData);
        }
    }
}

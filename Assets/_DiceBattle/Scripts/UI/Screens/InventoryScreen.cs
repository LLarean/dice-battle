using System;
using System.Collections.Generic;
using DiceBattle.Core;
using DiceBattle.Global;
using UnityEngine;
using System.Linq;
using DiceBattle.Data;

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
        [SerializeField] private InventoryItem _item;
        [SerializeField] private Transform _itemsSpawn;


        private void Start()
        {
            GenerateItems();
            ToggleReceivedStatus();
            ToggleEquippedMark();
            GenerateDice();
            SetUnitData();

            _diceHolder.Initialize(_dices);
            _diceHolder.RepositionDice();
        }

        private void OnEnable() => ToggleReceivedStatus();

        private void GenerateItems()
        {
            ClearItems();
            DiceList inventory = GameData.GetInventory();

            foreach (DiceType randomReward in inventory.DiceTypes)
            {
                InventoryItem inventoryItem = Instantiate(_item, _itemsSpawn);
                inventoryItem.Initialize(randomReward);
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

        private void ToggleReceivedStatus()
        {
            DiceList receivedRewards = GameData.GetInventory();

            foreach (InventoryItem item in _items)
            {
                bool isReceived = receivedRewards.DiceTypes.Contains(item.DiceType);
                item.gameObject.SetActive(isReceived);
            }
        }

        private void ToggleEquippedMark()
        {
            DiceList equippedRewards = GameData.GetEquippedItems();

            foreach (InventoryItem item in _items)
            {
                bool isReceived = equippedRewards.DiceTypes.Contains(item.DiceType);
                item.SetAgreeMark(isReceived);
            }
        }
    }
}

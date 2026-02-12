using System;
using System.Collections.Generic;
using DiceBattle.Core;
using DiceBattle.Global;
using UnityEngine;
using System.Linq;

namespace DiceBattle.UI
{
    public class InventoryScreen : Screen
    {
        private readonly List<Dice> _dices = new();
        private readonly List<InventoryItem> _items = new();

        [Space] [SerializeField] private UnitPanel _unitPanel;
        [SerializeField] private DiceHolder _diceHolder;
        [Space] [SerializeField] private Dice _dice;
        [SerializeField] private Transform _diceSpawn;
        [Space] [SerializeField] private InventoryItem _item;
        [SerializeField] private Transform _itemsSpawn;


        private void Start()
        {
            GenerateItems();
            ToggleItems();

            // GenerateDice();

            _diceHolder.Initialize(_dices);
            _diceHolder.RepositionDice();
        }

        private void OnEnable() => ToggleItems();

        private void GenerateItems()
        {
            ClearItems();
            RewardsData randomRewards = GameProgress.LoadRandomRewards();

            foreach (DiceType randomReward in randomRewards.DiceTypes)
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

            for (int i = 0; i < 6; i++)
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

        private void ToggleItems()
        {
            RewardsData receivedRewards = GameProgress.LoadReceivedRewards();

            foreach (InventoryItem item in _items)
            {
                bool isReceived = receivedRewards.DiceTypes.Contains(item.DiceType);
                item.gameObject.SetActive(isReceived);
            }
        }
    }
}

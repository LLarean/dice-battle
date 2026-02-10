using System;
using System.Collections.Generic;
using DiceBattle.Core;
using DiceBattle.Global;
using UnityEngine;
using System.Linq;

namespace DiceBattle.UI
{
    public class InventoryWindow : Screen
    {
        private readonly List<Dice> _dices = new();
        private readonly List<InventoryItem> _items = new();

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
            GenerateDice();
            GenerateItems();

            _diceHolder.Initialize(_dices);
            _diceHolder.RepositionDice();
            ToggleItems();
        }

        private void OnEnable() => ToggleItems();

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

        private void GenerateItems()
        {
            ClearItems();

            foreach (RewardType rewardType in Enum.GetValues(typeof(RewardType)))
            {
                InventoryItem inventoryItem = Instantiate(_item, _itemsSpawn);
                inventoryItem.Construct(rewardType);
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
            RewardsData receivedRewards = GameProgress.GetReceivedRewards();

            foreach (RewardType rewardType in Enum.GetValues(typeof(RewardType)))
            {
                ToggleItem(receivedRewards, rewardType);
            }
        }

        private void ToggleItem(RewardsData receivedRewards, RewardType rewardType)
        {
            InventoryItem inventoryItem = _items.FirstOrDefault(item => item.RewardType == rewardType);

            if (inventoryItem == null)
            {
                return;
            }

            if (receivedRewards.RewardTypes.Contains(rewardType))
            {
                inventoryItem.EnableMark();
            }
            else
            {
                inventoryItem.DisableMark();
            }
        }
    }
}

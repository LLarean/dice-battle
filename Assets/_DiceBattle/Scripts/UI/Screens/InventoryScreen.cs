using System.Collections.Generic;
using System.Linq;
using DiceBattle.Core;
using DiceBattle.Global;
using UnityEngine;
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
        }

        private void OnEnable()
        {
            RefreshInventoryDisplay();
        }

        private void RefreshInventoryDisplay()
        {
            GenerateItems();
            ToggleItems();
            _itemsPlaceholder.gameObject.SetActive(_items.Count == 0);

            GenerateDice();
            _diceHolder.Initialize(_dices);
            _diceHolder.RepositionDice();

            SetUnitData();
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
                bool isEquipped = equippedRewards.DiceTypes.Contains(item.DiceType);
                item.SetEquippedStatus(isEquipped);
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
                Title = "Герой (вы)",
                Portrait = _gameConfig.Player.Portraits[0],
                MaxHealth = GetHealth(),
                CurrentHealth = GetHealth(),
                Damage = GetDamage(),
                Armor = GetArmor(),
            };

            _unitPanel.SetUnitData(unitData);
        }

        private int GetHealth()
        {
            DiceList equippedRewards = GameData.GetEquippedItems();

            int health = _gameConfig.Player.StartHealth;
            int doubleHealthCount = equippedRewards.DiceTypes.Count(r => r == DiceType.BaseHealth);

            return health + doubleHealthCount;
        }

        private int GetDamage()
        {
            DiceList equippedRewards = GameData.GetEquippedItems();

            int damage = _gameConfig.Player.StartDamage;
            int baseDamageCount = equippedRewards.DiceTypes.Count(r => r == DiceType.BaseDamage);

            return damage + baseDamageCount;
        }

        private int GetArmor()
        {
            DiceList equippedRewards = GameData.GetEquippedItems();

            int armor = _gameConfig.Player.StartArmor;
            int baseArmorCount = equippedRewards.DiceTypes.Count(r => r == DiceType.BaseArmor);

            return armor + baseArmorCount;
        }
    }
}

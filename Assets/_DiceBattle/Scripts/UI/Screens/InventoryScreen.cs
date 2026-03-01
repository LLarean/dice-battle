using System.Collections.Generic;
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
        private readonly List<InventoryItem> _inventoryItems = new();

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

        // NOTE: Consider using object pooling here to reduce GC allocations
        // if this object is created/destroyed frequently
        private void RefreshInventoryDisplay()
        {
            GenerateItems();
            ToggleItems();
            _itemsPlaceholder.gameObject.SetActive(_inventoryItems.Count == 0);

            GenerateDice();
            _diceHolder.Initialize(_dices);
            _diceHolder.RepositionDice();

            SetUnitData();
        }

        private void GenerateItems()
        {
            ClearItems();
            List<Item> allItems = Inventory.AllItems();

            foreach (Item item in allItems)
            {
                InventoryItem inventoryItem = Instantiate(_item, _itemsSpawn);
                inventoryItem.Initialize(item);
                inventoryItem.OnDiceToggled += ItemClicked;
                _inventoryItems.Add(inventoryItem);
            }
        }

        private void ClearItems()
        {
            foreach (InventoryItem inventoryItem in _inventoryItems)
            {
                Destroy(inventoryItem.gameObject);
            }

            _inventoryItems.Clear();
        }

        private void ToggleItems()
        {
            List<Item> equippedItems = Inventory.EquippedItems();

            foreach (InventoryItem item in _inventoryItems)
            {
                bool isEquipped = equippedItems.Contains(item.Data);
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
            int health = _gameConfig.Player.StartHealth;
            int healthItemsCount = GameData.GetItemsCount(DiceType.BaseHealth);
            return health + healthItemsCount;
        }

        private int GetDamage()
        {
            int damage = _gameConfig.Player.StartDamage;
            int damageItemsCount = GameData.GetItemsCount(DiceType.BaseDamage);
            return damage + damageItemsCount;
        }

        private int GetArmor()
        {
            int armor = _gameConfig.Player.StartArmor;
            int armorItemsCount = GameData.GetItemsCount(DiceType.BaseArmor);
            return armor + armorItemsCount;
        }

        private void ItemClicked(DiceType obj)
        {
            throw new System.NotImplementedException();
        }

    }
}

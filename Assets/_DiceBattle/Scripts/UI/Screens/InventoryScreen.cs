using System.Collections.Generic;
using System.Linq;
using DiceBattle.Audio;
using DiceBattle.Core;
using DiceBattle.Data;
using DiceBattle.Events;
using GameSignals;
using UnityEngine;

namespace DiceBattle.UI
{
    public class InventoryScreen : Screen
    {
        private readonly List<InventoryItem> _inventoryItems = new();
        private readonly Dictionary<Dice, InventoryItem> _itemByDice = new();

        [SerializeField] private GameConfig _gameConfig;
        [Space]
        [SerializeField] private InventoryItem _item;
        [SerializeField] private Transform _availableSpawn;
        [SerializeField] private DiceHolder _deckHolder;

        private int DeckCapacity
        {
            get
            {
                int extra = Inventory.EquippedItems().Count(i => i.Type == DiceType.AdditionalDice);
                return _gameConfig.DiceStartCount + extra;
            }
        }

        private void OnEnable()
        {
            _deckHolder.OnSlotDiceClicked += HandleSlotDiceClicked;
            Refresh();
        }

        private void OnDisable() => _deckHolder.OnSlotDiceClicked -= HandleSlotDiceClicked;

        private void Refresh()
        {
            ClearItems();
            _deckHolder.Initialize(DeckCapacity);

            List<Item> allItems = Inventory.AllItems();

            foreach (Item item in allItems)
            {
                CreateItem(item);
            }

            foreach (InventoryItem inventoryItem in _inventoryItems.Where(i => i.Data.IsEquipped))
            {
                AddDiceCopyToHolder(inventoryItem);
            }
        }

        private void CreateItem(Item item)
        {
            InventoryItem inventoryItem = Instantiate(_item, _availableSpawn);
            inventoryItem.Initialize(item);
            inventoryItem.SetEquippedStatus(item.IsEquipped);
            inventoryItem.OnDiceToggled += _ => HandleCardClicked(inventoryItem);
            _inventoryItems.Add(inventoryItem);
        }

        private void ClearItems()
        {
            foreach (InventoryItem inventoryItem in _inventoryItems)
            {
                Destroy(inventoryItem.gameObject);
            }

            _inventoryItems.Clear();
            _itemByDice.Clear();
        }

        private void HandleCardClicked(InventoryItem inventoryItem)
        {
            Item item = inventoryItem.Data;

            if (item.IsEquipped)
            {
                Dice equippedCopy = _itemByDice.FirstOrDefault(pair => pair.Value == inventoryItem).Key;
                if (equippedCopy != null)
                {
                    Unequip(inventoryItem, equippedCopy);
                }

                return;
            }

            if (item.Type != DiceType.AdditionalDice && _deckHolder.FreeSlotCount == 0)
            {
                return;
            }

            Inventory.EquipItem(item);
            item.IsEquipped = true;
            inventoryItem.SetEquippedStatus(true);

            PlayClick();

            if (item.Type == DiceType.AdditionalDice)
            {
                Refresh();
                return;
            }

            AddDiceCopyToHolder(inventoryItem);
        }

        private void HandleSlotDiceClicked(Dice dice)
        {
            if (_itemByDice.TryGetValue(dice, out InventoryItem inventoryItem) == false)
            {
                return;
            }

            Unequip(inventoryItem, dice);
        }

        private void Unequip(InventoryItem inventoryItem, Dice copy)
        {
            Item item = inventoryItem.Data;
            Inventory.UnequipItem(item);
            item.IsEquipped = false;
            inventoryItem.SetEquippedStatus(false);

            _deckHolder.RemoveCopy(copy);
            _itemByDice.Remove(copy);

            PlayClick();

            if (item.Type == DiceType.AdditionalDice)
            {
                Refresh();
            }
        }

        private void AddDiceCopyToHolder(InventoryItem inventoryItem)
        {
            Dice copy = _deckHolder.TryEquipCopy(inventoryItem.Dice);

            if (copy != null)
            {
                _itemByDice[copy] = inventoryItem;
            }
            else
            {
                Inventory.UnequipItem(inventoryItem.Data);
                inventoryItem.Data.IsEquipped = false;
                inventoryItem.SetEquippedStatus(false);
            }
        }

        private void PlayClick() => SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
    }
}

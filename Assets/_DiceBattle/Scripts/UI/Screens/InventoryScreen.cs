using System.Collections.Generic;
using System.Linq;
using DiceBattle.Audio;
using DiceBattle.Data;
using DiceBattle.Events;
using DiceBattle.Global;
using GameSignals;
using TMPro;
using UnityEngine;

namespace DiceBattle.UI
{
    public class InventoryScreen : Screen
    {
        private readonly List<InventoryItem> _inventoryItems = new();

        [SerializeField] private GameConfig _gameConfig;
        [Space]
        [SerializeField] private TextMeshProUGUI _itemsPlaceholder;
        [Space]
        [SerializeField] private InventoryItem _item;
        [SerializeField] private Transform _deckSpawn;
        [SerializeField] private Transform _availableSpawn;

        private int DeckCapacity
        {
            get
            {
                int extra = Inventory.EquippedItems().Count(i => i.Type == DiceType.AdditionalDice);
                return _gameConfig.DiceStartCount + extra;
            }
        }

        private void OnEnable() => Refresh();

        private void Refresh()
        {
            ClearItems();

            List<Item> allItems = Inventory.AllItems();
            List<Item> equipped = allItems.Where(i => i.IsEquipped).ToList();
            List<Item> available = allItems.Where(i => !i.IsEquipped).ToList();

            foreach (Item item in equipped)
                CreateItem(item, _deckSpawn);

            foreach (Item item in available)
                CreateItem(item, _availableSpawn);

            if (_itemsPlaceholder != null)
                _itemsPlaceholder.gameObject.SetActive(available.Count == 0);
        }

        private void CreateItem(Item item, Transform parent)
        {
            InventoryItem inventoryItem = Instantiate(_item, parent);
            inventoryItem.Initialize(item);
            inventoryItem.SetEquippedStatus(item.IsEquipped);
            inventoryItem.OnDiceToggled += _ => HandleItemClicked(item);
            _inventoryItems.Add(inventoryItem);
        }

        private void ClearItems()
        {
            foreach (InventoryItem inventoryItem in _inventoryItems)
                Destroy(inventoryItem.gameObject);

            _inventoryItems.Clear();
        }

        private void HandleItemClicked(Item item)
        {
            if (item.IsEquipped)
            {
                Inventory.UnequipItem(item);
            }
            else
            {
                List<Item> equipped = Inventory.EquippedItems();

                if (item.Type == DiceType.AdditionalDice)
                {
                    Inventory.EquipItem(item);
                }
                else if (equipped.Count < DeckCapacity)
                {
                    Inventory.EquipItem(item);
                }
                else
                {
                    return;
                }
            }

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
            Refresh();
        }
    }
}

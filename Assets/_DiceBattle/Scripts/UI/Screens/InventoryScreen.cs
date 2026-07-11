using System;
using System.Collections.Generic;
using System.Linq;
using DiceBattle.Audio;
using DiceBattle.Core;
using DiceBattle.Data;
using DiceBattle.Events;
using DiceBattle.Global;
using GameSignals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class InventoryScreen : Screen
    {
        private readonly List<InventoryItem> _inventoryItems = new();
        private readonly Dictionary<Dice, InventoryItem> _itemByDice = new();

        [SerializeField] private GameConfig _gameConfig;
        [Space]
        [SerializeField] private UnitPanel _player;
        [SerializeField] private Button _previousCharacter;
        [SerializeField] private Button _nextCharacter;
        [SerializeField] private TMP_Text _diceCount;
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

        private void Start()
        {
            _previousCharacter.onClick.AddListener(PreviousCharacter);
            _nextCharacter.onClick.AddListener(NextCharacter);
        }

        private void OnDestroy()
        {
            _previousCharacter.onClick.RemoveAllListeners();
            _nextCharacter.onClick.RemoveAllListeners();
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

            UpdatePlayerPanel();
        }

        private void UpdatePlayerPanel()
        {
            UnitConfig playerConfig = _gameConfig.GetPlayerConfig(GameData.SelectedCharacterClass);
            UnitData playerData = HeroFactory.Build(playerConfig);
            playerData.Name = "Герой (вы)"; // TODO Translation
            playerData.Portrait = playerConfig.Portraits[0];

            _player.SetUnitData(playerData);

            List<Item> equippedItems = Inventory.EquippedItems();
            int armorBonus = equippedItems.Count(i => i.Type == DiceType.BaseArmor) * playerConfig.GrowthArmor;
            int damageBonus = equippedItems.Count(i => i.Type == DiceType.BaseDamage) * playerConfig.GrowthDamage;
            _player.SetEquipmentBonus(armorBonus, damageBonus);

            int used = DeckCapacity - _deckHolder.FreeSlotCount;
            _diceCount.text = $"{used} из {DeckCapacity}"; // TODO Translation
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
            UpdatePlayerPanel();
            RefreshMultipliers();
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
                return;
            }

            UpdatePlayerPanel();
            RefreshMultipliers();
        }

        private void RefreshMultipliers()
        {
            foreach (InventoryItem inventoryItem in _inventoryItems)
            {
                inventoryItem.RefreshMultiplier();
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

        private void PreviousCharacter() => ChangeCharacterClass(-1);

        private void NextCharacter() => ChangeCharacterClass(1);

        private void ChangeCharacterClass(int direction)
        {
            int classCount = _gameConfig.PlayerClasses.Length;
            int nextIndex = (((int)GameData.SelectedCharacterClass + direction) % classCount + classCount) % classCount;
            GameData.SelectedCharacterClass = (CharacterClass)nextIndex;

            PlayClick();
            Refresh();
        }

        private void PlayClick() => SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
    }
}

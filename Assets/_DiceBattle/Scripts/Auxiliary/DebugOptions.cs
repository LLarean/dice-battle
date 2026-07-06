#if UNITY_EDITOR
using System;
using DiceBattle.Global;
using DiceBattle.UI;
using NaughtyAttributes;
using UnityEngine;

namespace DiceBattle.Auxiliary
{
    public class DebugOptions : MonoBehaviour
    {
        [SerializeField] private DefaultInventory _defaultInventory;
        [Space]
        [SerializeField] private bool _needResetAll;
        [SerializeField] private bool _needAddAllItemsToInventory;
        [SerializeField] private DiceType _diceType;
        [Space]
        [SerializeField] private Item _item;

        private void Start()
        {
            if (_needResetAll)
            {
                GameData.ResetAll();
                GameSettings.ResetVolume();

                // _defaultInventory.SetEquippedItems();
                Inventory.Clear();
                _defaultInventory.SetDefaultInventory();
            }

            if (_needAddAllItemsToInventory)
            {
                AddAllItems();
            }
        }

        [Button]
        private void AddItem()
        {
            _item.IsEquipped = false;
            Inventory.AddItemToUnequipped(_item);
        }

        [Button]
        private void AddAndEquipItem()
        {
            _item.IsEquipped = true;
            Inventory.EquipItem(_item);
        }

        [Button]
        private void AddAllItems()
        {
            foreach (DiceType diceType in Enum.GetValues(typeof(DiceType)))
            {
                var item = new Item
                {
                    Type = diceType,
                    IsEquipped = false
                };

                Inventory.AddItemToUnequipped(item);
            }
        }

        [Button]
        private void AddDiceToInventory()
        {
            Inventory.AddItemToUnequipped(new Item { Type = _diceType, IsEquipped = false });
        }

        [Button]
        private void AddEquippedReward()
        {
            var item = new Item { Type = _diceType, IsEquipped = true };
            Inventory.AddItemToUnequipped(item);
            Inventory.EquipItem(item);
        }

        [Button]
        private void EquipAllItems()
        {
            foreach (Item item in Inventory.UnequippedItems())
            {
                Inventory.EquipItem(item);
            }
        }
    }
}
#endif

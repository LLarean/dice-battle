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
                AddAllRewards();
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

                Inventory.AddItemToUnequipped(_item);
            }
        }

        [Button]
        private void AddDiceToInventory()
        {
            DiceList inventory = GameData.GetInventory();
            inventory.DiceTypes.Add(_diceType);
            GameData.UpdateInventory(inventory);
        }

        [Button]
        private void AddEquippedReward()
        {
            DiceList equippedRewards = GameData.GetEquippedItems();
            equippedRewards.DiceTypes.Add(_diceType);
            GameData.SaveEquippedRewards(equippedRewards);
        }

        [Button]
        private void AddAllRewards()
        {
            DiceList inventory = GameData.GetInventory();
            DiceList randomRewards = GameData.LoadRandomRewards();

            foreach (DiceType randomReward in randomRewards.DiceTypes)
            {
                inventory.DiceTypes.Add(randomReward);
                // GameData.SaveInventory(randomReward);
            }

            GameData.UpdateInventory(inventory);
        }

        [Button]
        private void EquipAllRewards()
        {
            DiceList randomRewards = GameData.LoadRandomRewards();
            GameData.SaveEquippedRewards(randomRewards);
        }
    }
}
#endif

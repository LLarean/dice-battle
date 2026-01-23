using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class InventoryWindow : Screen
    {
        private readonly List<InventoryItem> _items = new();

        [Space]
        [SerializeField] private Transform _substrate;
        [SerializeField] private Button _close;
        [Space]
        [SerializeField] private InventoryItem _item;
        [SerializeField] private Transform _itemsSpawn;

        private void Awake()
        {
            GenerateItems();
        }

        private void Start()
        {
            _close.onClick.AddListener(HandleCloseClick);
        }

        private void OnDestroy() => _close.onClick.RemoveAllListeners();

        private void OnEnable() => ToggleItems();

        private void HandleCloseClick() => gameObject.SetActive(false);

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
            foreach (RewardType rewardType in Enum.GetValues(typeof(RewardType)))
            {
                ToggleItem(rewardType);
            }
        }

        private void ToggleItem(RewardType rewardType)
        {
            foreach (InventoryItem item in _items)
            {
                if (item.RewardType == rewardType)
                {
                    item.gameObject.SetActive(PlayerPrefs.GetInt(nameof(rewardType), 0) == 1);
                }
            }
        }
    }
}

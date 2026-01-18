using System.Collections.Generic;
using DiceBattle.Core;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class InventoryWindow : Screen
    {
        [SerializeField] private Transform _substrate;
        [SerializeField] private Button _close;
        [Space]
        [SerializeField] private List<Dice> _dices;
        [SerializeField] private List<InventoryItem> _items;

        private void Start()
        {
            _close.onClick.AddListener(HandleCloseClick);

            foreach (Dice dice in _dices)
            {
                dice.DisableButton();
            }

            gameObject.SetActive(false);
        }

        private void OnDestroy() => _close.onClick.RemoveAllListeners();

        private void OnEnable()
        {
            EnableDice();
            EnableItems();
        }

        private void HandleCloseClick() => gameObject.SetActive(false);

        private void EnableDice()
        {
            int availableDice = PlayerPrefs.GetInt("AvailableDice", 3);

            for (int i = 0; i < _dices.Count; i++)
            {
                _dices[i].gameObject.SetActive(i < availableDice);
            }
        }

        private void EnableItems()
        {
            ToggleRewardItem(RewardType.DisableEmptyState);
            ToggleRewardItem(RewardType.BaseDamage);
            ToggleRewardItem(RewardType.RegenHealth);
            ToggleRewardItem(RewardType.BaseArmor);
        }

        private void ToggleRewardItem(RewardType rewardType)
        {
            if (TryGetRewardItem(rewardType, out InventoryItem item))
            {
                item.gameObject.SetActive(PlayerPrefs.GetInt(nameof(rewardType), 0) == 1);
            }
        }

        private bool TryGetRewardItem(RewardType rewardType, out InventoryItem inventoryItem)
        {
            inventoryItem = null;

            foreach (InventoryItem item in _items)
            {
                if (item.RewardType == rewardType)
                {
                    inventoryItem = item;
                    return true;
                }
            }

            return false;
        }
    }
}

using System.Collections.Generic;
using DiceBattle.Audio;
using DiceBattle.Core;
using GameSignals;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.Windows
{
    public class InventoryWindow : MonoBehaviour, IInventoryWindowHandler
    {
        [SerializeField] private Transform _substrate;
        [SerializeField] private Button _close;
        [Space]
        [SerializeField] private List<Dice> _dices;
        [SerializeField] private List<InventoryItem> _items;

        public void Show() => _substrate.gameObject.SetActive(true);

        public void Hide() => _substrate.gameObject.SetActive(false);

        private void Start()
        {
            _close.onClick.AddListener(HandleCloseClick);
            Hide();
            SignalSystem.Subscribe(this);

            foreach (Dice dice in _dices)
            {
                dice.DisableButton();
            }
        }

        private void OnDestroy()
        {
            _close.onClick.RemoveAllListeners();
            SignalSystem.Unsubscribe(this);
        }

        private void OnEnable()
        {
            EnableDice();
            EnableItems();
        }

        private void HandleCloseClick() => Hide();

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
            ToggleRewardItem(RewardType.DoubleDamage);
            ToggleRewardItem(RewardType.HealthRegen);
            ToggleRewardItem(RewardType.Armor);
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

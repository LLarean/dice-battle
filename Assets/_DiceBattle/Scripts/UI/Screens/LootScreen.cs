using System.Collections.Generic;
using DiceBattle.Audio;
using DiceBattle.Events;
using DiceBattle.Global;
using GameSignals;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class LootScreen : Screen
    {
        [SerializeField] private List<InventoryItem> _rewardItems;
        [SerializeField] private Button _reroll;

        private readonly List<DiceType> _currentRewards = new();

        private void Start()
        {
            foreach (InventoryItem rewardItem in _rewardItems)
            {
                rewardItem.OnDiceToggled += HandleItemSelect;
            }

            _reroll.onClick.AddListener(RerollRewards);
        }

        private void OnDestroy()
        {
            foreach (InventoryItem rewardItem in _rewardItems)
            {
                rewardItem.OnDiceToggled -= HandleItemSelect;
            }

            _reroll.onClick.RemoveAllListeners();
        }

        private void OnEnable()
        {
            ShowRewards();
        }

        private void HandleItemSelect(DiceType diceType)
        {
            Inventory.AddItemToUnequipped(new Item { Type = diceType, IsEquipped = false });

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Reward));

            gameObject.SetActive(false);
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.TavernScreen));
        }

        private void ShowRewards()
        {
            DiceList randomRewards = GameData.LoadRandomRewards();
            GameData.SaveRandomRewards(randomRewards);
            GameData.LogRandomRewards();

            int firstRewardCount = GameData.CompletedLevels;
            int secondRewardCount = GameData.CompletedLevels + 1;

            _currentRewards.Clear();
            _currentRewards.Add(randomRewards.DiceTypes[firstRewardCount]);
            _currentRewards.Add(randomRewards.DiceTypes[secondRewardCount]);

            for (int i = 0; i < _rewardItems.Count; i++)
            {
                _rewardItems[i].Initialize(new Item { Type = _currentRewards[i], IsEquipped = false });
                _rewardItems[i].SetEquippedStatus(false);
            }
        }

        private void RerollRewards() => ShowRewards();
    }
}

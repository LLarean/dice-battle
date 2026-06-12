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
        [SerializeField] private List<RewardItem> _rewardItems;
        [SerializeField] private Button _reroll;

        private void Start()
        {
            foreach (RewardItem rewardItem in _rewardItems)
            {
                rewardItem.OnClicked += HandleItemSelect;
            }

            _reroll.onClick.AddListener(RerollRewards);
        }

        private void OnDestroy()
        {
            foreach (RewardItem rewardItem in _rewardItems)
            {
                rewardItem.OnClicked -= HandleItemSelect;
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

            _rewardItems[0].SetReward(randomRewards.DiceTypes[firstRewardCount]);
            _rewardItems[1].SetReward(randomRewards.DiceTypes[secondRewardCount]);
        }

        private void RerollRewards() => ShowRewards();
    }
}

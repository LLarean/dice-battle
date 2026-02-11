using System.Collections.Generic;
using DiceBattle.Audio;
using DiceBattle.Events;
using DiceBattle.Global;
using GameSignals;
using UnityEngine;

namespace DiceBattle.UI
{
    public class LootScreen : Screen
    {
        [SerializeField] private List<RewardItem> _rewardItems;

        private void Start()
        {
            foreach (RewardItem rewardItem in _rewardItems)
            {
                rewardItem.OnClicked += HandleItemSelect;
            }
        }

        private void OnDestroy()
        {
            foreach (RewardItem rewardItem in _rewardItems)
            {
                rewardItem.OnClicked -= HandleItemSelect;
            }
        }

        private void OnEnable()
        {
            ShowRewards();
        }

        private void HandleItemSelect(DiceType diceType)
        {
            GameProgress.AddReceivedReward(diceType);
            GameProgress.LogReceivedReward();

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Reward));
            SignalSystem.Raise<IChangeHandler>(handler => handler.UpdateRewards());

            gameObject.SetActive(false);
        }

        private void ShowRewards()
        {
            RewardsData randomRewards = GameProgress.GetRandomRewards();
            GameProgress.AddRandomRewards(randomRewards);
            GameProgress.LogRandomRewards();

            int firstRewardCount = GameProgress.CompletedLevels;
            int secondRewardCount = GameProgress.CompletedLevels + 1;

            _rewardItems[0].SetReward(randomRewards.DiceTypes[firstRewardCount]);
            _rewardItems[1].SetReward(randomRewards.DiceTypes[secondRewardCount]);
        }
    }
}

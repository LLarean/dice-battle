using System;
using System.Collections.Generic;
using System.Linq;
using DiceBattle.Audio;
using DiceBattle.Events;
using DiceBattle.Global;
using DiceBattle.UI;
using GameSignals;
using UnityEngine;
using Random = System.Random;

namespace DiceBattle.UI
{
    public class LootScreen : Screen
    {
        [SerializeField] private List<RewardItem> _rewardItems;

        private Random _random;

        private void Awake() => _random = new Random();

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

        private void OnEnable() => RollReward();

        private void HandleItemSelect(RewardType rewardType)
        {
            GameProgress.AddReceivedReward(rewardType);
            GameProgress.LogReceivedReward();

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Reward));
            SignalSystem.Raise<IChangeHandler>(handler => handler.UpdateRewards());

            gameObject.SetActive(false);
        }

        private void RollReward()
        {
            RewardsData rewardsData = GameProgress.GetRandomRewards();

            if (rewardsData.RewardTypes.Count != 0)
            {
                return;
            }

            var allRewardTypes = Enum.GetValues(typeof(RewardType)).Cast<RewardType>().ToList();
            var newRewards = allRewardTypes.OrderBy(x => _random.Next()).ToList();
            rewardsData.RewardTypes = newRewards;
            GameProgress.AddRandomRewards(rewardsData);
            GameProgress.LogRandomRewards();

            var allValues = (RewardType[])Enum.GetValues(typeof(RewardType));

            foreach (RewardItem rewardItem in _rewardItems)
            {
                int randomIndex = _random.Next(0, allValues.Length);
                var reward = (RewardType)randomIndex;

                rewardItem.SetReward(reward);
            }
        }
    }
}

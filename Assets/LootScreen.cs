using System;
using System.Collections.Generic;
using DiceBattle.Audio;
using GameSignals;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace DiceBattle
{
    public class LootScreen : MonoBehaviour
    {
        [SerializeField] private List<RewardItem> _rewardItems;

        private Random _random;

        public void RollReward()
        {
            var allValues = (RewardType[])Enum.GetValues(typeof(RewardType));

            foreach (RewardItem rewardItem in _rewardItems)
            {
                int randomIndex = _random.Next(0, allValues.Length);
                var reward = (RewardType)randomIndex;

                rewardItem.SetReward(reward);
            }
        }

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
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));

            PlayerPrefs.SetInt(rewardType.ToString(), 1);

            gameObject.SetActive(false);
        }
    }
}

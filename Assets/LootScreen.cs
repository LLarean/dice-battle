using System;
using System.Collections.Generic;
using DiceBattle.Audio;
using GameSignals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace DiceBattle
{
    public class LootScreen : MonoBehaviour
    {
        [SerializeField] private List<RewardItem> _rewardItem;
        [SerializeField] private Button _take;

        private Random _random;

        public void RollReward()
        {
            var allValues = (RewardType[])Enum.GetValues(typeof(RewardType));
            int randomIndex = _random.Next(0, allValues.Length);
            var rewardType = (RewardType)randomIndex;
            _rewardItem[0].SetReward(rewardType);

            randomIndex = _random.Next(0, allValues.Length);
            rewardType = (RewardType)randomIndex;
            _rewardItem[1].SetReward(rewardType);
        }

        private void Awake() => _random = new Random();

        private void Start() => _take.onClick.AddListener(HandleTakeClick);

        private void OnDestroy() => _take.onClick.RemoveAllListeners();

        private void OnEnable() => RollReward();

        private void HandleTakeClick()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));

            gameObject.SetActive(false);
        }
    }
}

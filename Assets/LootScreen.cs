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
        [SerializeField] private List<RewardItem> _rewardItem;
        [SerializeField] private Button _take;

        private Random _random;
        private RewardType _firstReward;
        private RewardType _secondReward;

        public void RollReward()
        {
            var allValues = (RewardType[])Enum.GetValues(typeof(RewardType));
            int randomIndex = _random.Next(0, allValues.Length);
            _firstReward = (RewardType)randomIndex;
            _rewardItem[0].SetReward(_firstReward);

            randomIndex = _random.Next(0, allValues.Length);
            _secondReward = (RewardType)randomIndex;
            _rewardItem[1].SetReward(_secondReward);
        }

        private void Awake() => _random = new Random();

        private void Start()
        {
            _take.onClick.AddListener(HandleTakeClick);
            _rewardItem[0].OnClicked += SelectFirst;
            _rewardItem[1].OnClicked += SelectSecond;
        }

        private void OnDestroy()
        {
            _take.onClick.RemoveAllListeners();
            _rewardItem[0].OnClicked -= SelectFirst;
            _rewardItem[1].OnClicked -= SelectSecond;
        }

        private void OnEnable() => RollReward();

        private void HandleTakeClick()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));

            gameObject.SetActive(false);
        }

        private void SelectFirst()
        {
            HandleTakeClick();
            PlayerPrefs.SetInt(_firstReward.ToString(), 1);
        }

        private void SelectSecond()
        {
            HandleTakeClick();
            PlayerPrefs.SetInt(_secondReward.ToString(), 1);
        }
    }
}

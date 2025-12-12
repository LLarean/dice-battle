using System;
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
        [SerializeField] private TextMeshProUGUI _reward;
        [SerializeField] private Button _next;

        private Random _random;

        private void Start()
        {
            _random = new Random();

            _next.onClick.AddListener(HandleStartClick);
        }

        private void OnDestroy() => _next.onClick.RemoveAllListeners();

        private void OnEnable() => RollReward();

        [ContextMenu("RollReward")]
        private void RollReward()
        {
            var allValues = (RewardType[])Enum.GetValues(typeof(RewardType));
            int randomIndex = _random.Next(0, allValues.Length);
            var rewardType = (RewardType)randomIndex;
            _reward.text = rewardType.ToString();
        }

        private void HandleStartClick()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));

            gameObject.SetActive(false);
        }
    }
}

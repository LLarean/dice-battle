using System.Collections.Generic;
using DiceBattle.Animations;
using DiceBattle.Audio;
using DiceBattle.Core;
using DiceBattle.Events;
using DiceBattle.Global;
using GameSignals;
using TMPro;
using UnityEngine;

namespace DiceBattle.UI
{
    public class LootScreen : Screen
    {
        [SerializeField] private TMP_Text _message;
        [SerializeField] private List<Dice> _dice;
        [SerializeField] private List<InventoryItem> _rewardItems;
        [SerializeField] private RectTransform _rollAnimationArea;

        private const float _flyDuration = 0.35f;
        private const float _flyStagger = 0.08f;

        private readonly List<DiceType> _currentRewards = new();

        private bool _isRolling;

        private void Start()
        {
            foreach (InventoryItem rewardItem in _rewardItems)
            {
                rewardItem.OnDiceToggled += HandleItemSelect;
            }

            DiceAnimation.OnDiceRollComplete += HandleRollComplete;
        }

        private void OnDestroy()
        {
            foreach (InventoryItem rewardItem in _rewardItems)
            {
                rewardItem.OnDiceToggled -= HandleItemSelect;
            }

            DiceAnimation.OnDiceRollComplete -= HandleRollComplete;
            LeanTween.cancel(gameObject);
        }

        private void OnEnable()
        {
            PrepareRewards();
            StartRewardRoll();
        }

        private void HandleItemSelect(DiceType diceType)
        {
            Inventory.AddItemToUnequipped(new Item { Type = diceType, IsEquipped = false });

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Reward));

            gameObject.SetActive(false);
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.TavernScreen));
        }

        private void PrepareRewards()
        {
            int startIndex = GameData.CompletedLevels;

            _currentRewards.Clear();
            _currentRewards.AddRange(GameData.GetRandomRewards(startIndex, _rewardItems.Count));
            GameData.LogRandomRewards();

            _message.gameObject.SetActive(false);

            for (int i = 0; i < _rewardItems.Count; i++)
            {
                _rewardItems[i].Initialize(new Item { Type = _currentRewards[i], IsEquipped = false });
                _rewardItems[i].SetEquippedStatus(false);
                _rewardItems[i].SetContentVisible(false);
            }
        }

        private void StartRewardRoll()
        {
            LeanTween.cancel(gameObject);
            _isRolling = true;

            foreach (Dice dice in _dice)
            {
                dice.transform.SetParent(_rollAnimationArea);
                dice.gameObject.SetActive(true);
            }

            DiceAnimation.Animate(_dice, _rollAnimationArea);
        }

        private void HandleRollComplete()
        {
            if (_isRolling == false)
            {
                return;
            }

            for (int i = 0; i < _dice.Count; i++)
            {
                _dice[i].SetFixedFace(_currentRewards[i].GetIconCategory());
            }

            FlyDiceToCards();
        }

        private void FlyDiceToCards()
        {
            int remaining = _dice.Count;

            for (int i = 0; i < _dice.Count; i++)
            {
                int index = i;
                Vector3 targetPosition = _rewardItems[index].Dice.transform.position;

                LeanTween.move(_dice[index].gameObject, targetPosition, _flyDuration)
                    .setDelay(index * _flyStagger)
                    .setEase(LeanTweenType.easeInOutQuad)
                    .setOnComplete(() =>
                    {
                        _dice[index].gameObject.SetActive(false);
                        _rewardItems[index].SetContentVisible(true);

                        remaining--;
                        if (remaining == 0)
                        {
                            _message.gameObject.SetActive(true);
                            _isRolling = false;
                        }
                    });
            }
        }
    }
}

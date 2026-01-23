using System;
using System.Collections.Generic;
using DiceBattle.Animations;
using DiceBattle.Audio;
using DiceBattle.Events;
using DiceBattle.UI;
using GameSignals;
using UnityEngine;

namespace DiceBattle.Core
{
    public class DiceShaker : MonoBehaviour
    {
        [SerializeField] private Hint _hint;
        [SerializeField] private RectTransform _rollArea;

        private DiceAnimation _diceAnimation;

        public event Action OnRollCompleted;

        public void ShowAttemptsHint(int attemptCount) => _hint.ShowAttempts(attemptCount);

        public void Roll(List<Dice> dices)
        {
            foreach (Dice dice in dices)
            {
                dice.transform.SetParent(_rollArea);
                dice.transform.localPosition = Vector3.zero;
            }

            _diceAnimation.Animate(dices);
            PlaySound(dices);
        }

        private static void PlaySound(List<Dice> dices)
        {
            SoundType soundType = dices.Count > 1 ? SoundType.DiceThrow : SoundType.DieThrow;
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(soundType));
        }

        private void HandleRollComplete() => OnRollCompleted?.Invoke();

        private void Awake() => _diceAnimation = new DiceAnimation(_rollArea);

        private void Start() => _diceAnimation.OnDiceRollComplete += HandleRollComplete;

        private void OnDestroy() => _diceAnimation.OnDiceRollComplete -= HandleRollComplete;
    }
}

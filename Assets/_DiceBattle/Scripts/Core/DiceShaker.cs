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
        [SerializeField] private RectTransform _rollArea;

        public event Action OnRollCompleted;

        public void Roll(List<Dice> dices)
        {
            foreach (Dice dice in dices)
            {
                dice.transform.SetParent(_rollArea);
                dice.transform.localPosition = Vector3.zero;
            }

            DiceAnimation.Animate(dices, _rollArea);
            PlaySound(dices);
        }

        private static void PlaySound(List<Dice> dices)
        {
            SoundType soundType = dices.Count > 1 ? SoundType.DiceThrow : SoundType.DieThrow;
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(soundType));
        }

        private void HandleRollComplete() => OnRollCompleted?.Invoke();

        private void Start() => DiceAnimation.OnDiceRollComplete += HandleRollComplete;

        private void OnDestroy() => DiceAnimation.OnDiceRollComplete -= HandleRollComplete;
    }
}

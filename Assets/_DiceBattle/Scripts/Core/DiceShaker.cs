using System;
using System.Collections.Generic;
using DiceBattle.Audio;
using GameSignals;
using UnityEngine;

namespace DiceBattle.Core
{
    public class DiceShaker : MonoBehaviour
    {
        [SerializeField] private DiceRollAnimation _diceRollAnimation;
        [SerializeField] private Transform _rollArea;

        public event Action OnRollCompleted;

        public void Roll(List<Dice> dices)
        {
            foreach (Dice dice in dices)
            {
                dice.transform.SetParent(_rollArea);
                dice.transform.localPosition = Vector3.zero;
            }

            _diceRollAnimation.RollDice(dices);
            PlaySound(dices);
        }

        private static void PlaySound(List<Dice> dices)
        {
            if (dices.Count > 1)
            {
                SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.DiceThrow));
            }
            else
            {
                SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.DieThrow));
            }
        }

        private void HandleRollComplete() => OnRollCompleted?.Invoke();

        private void Start()
        {
            _diceRollAnimation.OnDiceRollComplete += HandleRollComplete;
        }

        private void OnDestroy()
        {
            _diceRollAnimation.OnDiceRollComplete -= HandleRollComplete;
        }
    }
}

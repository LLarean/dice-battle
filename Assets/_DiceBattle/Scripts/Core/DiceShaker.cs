using System;
using System.Collections.Generic;
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

            _diceRollAnimation.RollDices(dices);
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

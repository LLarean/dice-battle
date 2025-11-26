using System;
using System.Collections.Generic;
using DiceBattle.Core;
using UnityEngine;

namespace DiceBattle
{
    public class DicePanel : MonoBehaviour
    {
        [SerializeField] private Hint _hint;
        [SerializeField] private List<Dice> _dices;

        public List<Dice> Dices => _dices;
        
        public event Action OnDiceClicked;

        public void Initialize()
        {
            foreach (var dice in _dices)
            {
                dice.Reset();
                dice.DisableInteractable();
            }
            
            // _hint.ShowAttempts(3);
        }
        
        public void RollAllDice()
        {
            foreach (var dice in _dices)
            {
                dice.Unlock();
                dice.Roll();
            }
        }

        public void RollUnlockedDice()
        {
            foreach (var dice in _dices)
            {
                if (dice.IsLocked == false)
                {
                    dice.Roll();
                }
            }
        }
        
        public void EnableInteractable()
        {
            foreach (var dice in _dices)
            {
                dice.EnableInteractable();
            }
        }
        
        public void DisableInteractable()
        {
            foreach (var dice in _dices)
            {
                dice.DisableInteractable();
            }
        }

        public void ShowAttempts(int attemptCount) => _hint.ShowAttempts(attemptCount);

        private void Start()
        {
            foreach (var dice in _dices)
            {
                dice.OnClicked += DiceClick;
            }
        }

        private void OnDestroy()
        {
            foreach (var dice in _dices)
            {
                dice.OnClicked -= DiceClick;
            }
        }

        private void DiceClick() => OnDiceClicked?.Invoke();
    }
}
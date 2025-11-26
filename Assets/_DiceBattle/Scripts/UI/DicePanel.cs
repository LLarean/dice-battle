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
        
        public void Initialize()
        {
            foreach (var dice in _dices)
            {
                dice.Reset();
                dice.DisableInteractable();
            }
            
            _hint.ShowAttempts(3);
        }
        
        public void RollAllDice()
        {
            foreach (var dice in _dices)
            {
                dice.Unlock();
                dice.Roll();
            }
        }

        public void RerollUnlockedDice()
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
            foreach (var diceUI in _dices)
            {
                diceUI.EnableInteractable();
            }
        }
    }
}
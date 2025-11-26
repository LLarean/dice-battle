using System.Collections.Generic;
using DiceBattle.Data;
using DiceBattle.UI;
using UnityEngine;

namespace DiceBattle
{
    public class DicePanel : MonoBehaviour
    {
        [SerializeField] private GameConfig _config;
        [Space]
        [SerializeField] private List<DiceUI> _diceUIList;

        public List<DiceUI> Dices => _diceUIList;
        
        public void Initialize()
        {
        }
        
        public void RollAllDice()
        {
            foreach (var dice in _diceUIList)
            {
                dice.Unlock();
                dice.Roll();
            }
        }

        public void RerollUnlockedDice()
        {
            foreach (var dice in _diceUIList)
            {
                if (dice.IsLocked == false)
                {
                    dice.Roll();
                }
            }
        }
        
        public void UpdateDiceVisuals()
        {
        }

        public void EnableInteractable()
        {
            foreach (var diceUI in _diceUIList)
            {
                diceUI.EnableInteractable();
            }
        }
    }
}
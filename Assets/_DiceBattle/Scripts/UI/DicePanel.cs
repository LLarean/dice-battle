using System.Collections.Generic;
using DiceBattle.Core;
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

        private List<Dice> _dices;

        public List<Dice> Dices => _dices;
        
        public void Initialize()
        {
            _dices = new List<Dice>();
            
            for (int i = 0; i < _config.DiceCount; i++)
            {
                _dices.Add(new Dice());
            }
            
            for (int i = 0; i < _diceUIList.Count && i < _dices.Count; i++)
            {
                _diceUIList[i].Initialize(_dices[i], _config.DiceSprites);
            }
        }
        
        public void RollAllDice()
        {
            foreach (var dice in _dices)
            {
                dice.Unlock();
                dice.Roll();
            }

            UpdateDiceVisuals();
        }

        public void RerollUnlockedDice()
        {
            foreach (var dice in _dices)
            {
                if (!dice.IsLocked)
                {
                    dice.Roll();
                }
            }

            UpdateDiceVisuals();
        }
        
        public void UpdateDiceVisuals()
        {
            for (int i = 0; i < _diceUIList.Count && i < _dices.Count; i++)
            {
                _diceUIList[i].UpdateVisual();
            }
        }

        public void EnableInteractable()
        {
            foreach (var diceUI in _diceUIList)
            {
                diceUI.SetInteractable(false);
            }
        }
    }
}
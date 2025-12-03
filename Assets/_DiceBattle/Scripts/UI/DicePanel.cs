using System;
using System.Collections.Generic;
using DiceBattle.Core;
using UnityEngine;

namespace DiceBattle
{
    public class DicePanel : MonoBehaviour
    {
        [SerializeField] private DiceTray diceTray;
        [SerializeField] private DiceRollAnimation _diceRollAnimation;
        [Space]
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

            diceTray.SetAllDices(_dices.ToArray());
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

        public void UnlockAll()
        {
            foreach (var dice in _dices)
            {
                dice.Unlock();
            }
        }

        public void RollUnlockedDice()
        {
            var unclockedDices = GetUnlockedDices();

            SetPosition(unclockedDices.ToArray());
            _diceRollAnimation.RollAllDices();
            
            foreach (var dice in unclockedDices)
            {
                dice.Roll();
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

        private void SetPosition(Dice[] dices)
        {
            for (int i = 0; i < dices.Length; i++)
            {
                dices[i].transform.SetParent(transform);
                dices[i].transform.localPosition = Vector3.zero;
            }
        }

        private List<Dice> GetUnlockedDices()
        {
            List<Dice> dices = new List<Dice>();

            foreach (var dice in _dices)
            {
                if (dice.IsLocked == false)
                {
                    dices.Add(dice);
                }
            }
            
            return dices;
        }
    }
}
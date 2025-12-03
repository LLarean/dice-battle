using System;
using System.Collections.Generic;
using DiceBattle.Core;
using UnityEngine;

namespace DiceBattle
{
    public class DicePanel : MonoBehaviour
    {
        [SerializeField] private DiceHolder _diceHolder;
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
                dice.ResetToEmpty();
                dice.DisableButton();
            }

            _diceHolder.PlaceSet(_dices);
            // _hint.ShowAttempts(3);
        }

        public void UnlockAll()
        {
            foreach (var dice in _dices)
            {
                dice.ClearSelection();
            }
        }

        public void RollUnlockedDice()
        {
            List<Dice> unclockedDices = _diceHolder.GetUnlockedDices();

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
                dice.EnableButton();
            }
        }

        public void DisableInteractable()
        {
            foreach (var dice in _dices)
            {
                dice.DisableButton();
            }
        }

        public void ShowAttempts(int attemptCount) => _hint.ShowAttempts(attemptCount);

        private void Start()
        {
            foreach (var dice in _dices)
            {
                dice.OnClicked += DiceClick;
            }

            _diceRollAnimation.OnDiceRollComplete += ReturnDiceToHolder;
        }

        private void OnDestroy()
        {
            foreach (var dice in _dices)
            {
                dice.OnClicked -= DiceClick;
            }

            _diceRollAnimation.OnDiceRollComplete -= ReturnDiceToHolder;
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

        private void ReturnDiceToHolder()
        {
            _diceHolder.PlaceSet(_dices);
        }

    }
}

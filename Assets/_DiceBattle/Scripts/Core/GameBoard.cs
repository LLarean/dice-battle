using System;
using System.Collections.Generic;
using UnityEngine;

namespace DiceBattle.Core
{
    public class GameBoard : MonoBehaviour
    {
        [SerializeField] private DiceShaker _diceShaker;
        [SerializeField] private DiceHolder _diceHolder;

        public event Action OnRollCompleted;
        public event Action OnDiceToggled;

        public bool HasSelectedDice => _diceHolder.Selected.Count > 0;

        public void RollAllDice() => _diceShaker.Roll(_diceHolder.Occupied);

        public void RerollSelectedDice() => _diceShaker.Roll(_diceHolder.Selected);

        public void GetRollResult()
        {
            // TODO: to be implemented later
        }

        #region Event Handlers

        private void HandleRollComplete(List<Dice> rolledDices)
        {
            _diceHolder.PlaceSet(rolledDices);
            OnRollCompleted?.Invoke();
        }

        private void HandleDiceToggle() => OnDiceToggled?.Invoke();

        #endregion

        #region Unity lifecycle

        private void Start()
        {
            _diceShaker.OnRollCompleted += HandleRollComplete;
            _diceHolder.OnDiceToggled += HandleDiceToggle;
        }

        private void OnDestroy()
        {
            _diceShaker.OnRollCompleted -= HandleRollComplete;
            _diceHolder.OnDiceToggled -= HandleDiceToggle;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DiceBattle.Core
{
    public class GameBoard : MonoBehaviour
    {
        [SerializeField] private DiceShaker _diceShaker;
        [SerializeField] private DiceHolder _diceHolder;
        [Space]
        [SerializeField] private List<Dice> _dices;

        public event Action OnRollCompleted;
        public event Action OnDiceToggled;

        public List<Dice> Dices => _dices;

        public void RollAllDice() => _diceShaker.Roll(_diceHolder.Occupied);

        public void RerollSelectedDice() => _diceShaker.Roll(_diceHolder.Selected);

        public void EnableAllDice() => _dices.ForEach(dice => dice.EnableButton());

        public void DisableAllDice() => _dices.ForEach(dice => dice.DisableButton());

        public void ClearAllSelection() => _dices.ForEach(dice => dice.ClearSelection());

        #region Event Handlers

        private void HandleRollComplete()
        {
            _dices.ForEach(dice => dice.Roll());
            _diceHolder.RepositionDice();
            OnRollCompleted?.Invoke();
        }

        private void HandleDiceToggle() => OnDiceToggled?.Invoke();

        #endregion

        #region Unity lifecycle

        private void Start()
        {
            _diceShaker.OnRollCompleted += HandleRollComplete;
            _diceHolder.OnDiceToggled += HandleDiceToggle;

            _diceHolder.Initialize(_dices);
        }

        private void OnDestroy()
        {
            _diceShaker.OnRollCompleted -= HandleRollComplete;
            _diceHolder.OnDiceToggled -= HandleDiceToggle;
        }

        #endregion
    }
}

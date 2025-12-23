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

        public void RollDice() => _diceShaker.Roll(_diceHolder.Occupied);

        public void RerollSelectedDice() => _diceShaker.Roll(_diceHolder.Selected);

        public void EnableDiceInteractable() => _dices.ForEach(dice => dice.EnableButton());

        public void DisableDiceInteractable() => _dices.ForEach(dice => dice.DisableButton());

        public void ClearAllSelection() => _dices.ForEach(dice => dice.ClearSelection());

        public void SetDiceCount(int count)
        {
            for (int i = 0; i < _dices.Count; i++)
            {
                _dices[i].gameObject.SetActive(i < count);
            }

            _diceHolder.SetSocketCount(count);
        }

        #region Event Handlers

        private void HandleRollComplete()
        {
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

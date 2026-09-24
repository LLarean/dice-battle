using System;
using System.Collections.Generic;
using DiceBattle.Data;
using UnityEngine;

namespace DiceBattle.Core
{
    public class GameBoard : MonoBehaviour
    {
        private readonly List<Dice> _dices = new();

        [SerializeField] private GameConfig _config;
        [Space]
        [SerializeField] private DiceShaker _diceShaker;
        [SerializeField] private DiceHolder _diceHolder;
        [Space]
        [SerializeField] private Dice _dice;
        [SerializeField] private Transform _diceSpawn;

        public event Action OnRollCompleted;
        public event Action OnDiceToggled;

        public List<Dice> Dices => _dices;
        public bool HaveSelectedDice => _diceHolder.Selected.Count > 0;
        public bool HaveUnselectedDice => _diceHolder.Selected.Count < _dices.Count;
        public bool DiceInteractable => _dices.Count > 0 && _dices[0].Interactable;

        public void RollDice() => _diceShaker.Roll(_diceHolder.Occupied);

        public void RerollSelectedDice() => _diceShaker.Roll(_diceHolder.Selected);

        public void EnableDiceInteractable() => _dices.ForEach(dice => dice.EnableButton());

        public void DisableDiceInteractable() => _dices.ForEach(dice => dice.DisableButton());

        public void ClearAllSelection() => _dices.ForEach(dice => dice.ClearSelection());

        public void ResetDice() => _dices.ForEach(dice => dice.ResetToEmpty());

        public void ToggleAll() => _dices.ForEach(dice => dice.Toggle());

        public void SetSelectionStatus(bool isSelected) => _dices.ForEach(dice => dice.SetSelection(isSelected));

        public void SetDeck(List<DiceType> deck)
        {
            if (deck.Count != _dices.Count)
            {
                RebuildDice(deck.Count);
            }

            for (int i = 0; i < deck.Count; i++)
            {
                _dices[i].SetType(deck[i]);
            }
        }

        private void HandleRollComplete()
        {
            _diceHolder.AnimateDiceToSlots(() => OnRollCompleted?.Invoke());
        }

        private void HandleDiceToggle() => OnDiceToggled?.Invoke();

        // GameScreen.OnEnable may run before this Awake and have built the dice already.
        private void Awake()
        {
            if (_dices.Count == 0)
            {
                SetDeck(DiceRuleset.Deck(_config.DiceStartCount));
            }
        }

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

        private void RebuildDice(int diceCount)
        {
            ClearDice();

            for (int i = 0; i < diceCount; i++)
            {
                Dice dice = Instantiate(_dice, _diceSpawn);
                _dices.Add(dice);
            }

            _diceHolder.Initialize(_dices);
        }

        private void ClearDice()
        {
            foreach (Dice dice in _dices)
            {
                Destroy(dice.gameObject);
            }

            _dices.Clear();
        }
    }
}

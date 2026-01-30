using System;
using System.Collections.Generic;
using System.Linq;
using DiceBattle.Data;
using DiceBattle.Global;
using DiceBattle.UI;
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

        public void RollDice() => _diceShaker.Roll(_diceHolder.Occupied);

        public void RerollSelectedDice() => _diceShaker.Roll(_diceHolder.Selected);

        public void EnableDiceInteractable() => _dices.ForEach(dice => dice.EnableButton());

        public void DisableDiceInteractable() => _dices.ForEach(dice => dice.DisableButton());

        public void ClearAllSelection() => _dices.ForEach(dice => dice.ClearSelection());

        public void ToggleAll() => _dices.ForEach(dice => dice.Toggle());

        public void SetDiceCount(int count)
        {
            for (int i = 0; i < _dices.Count; i++)
            {
                _dices[i].gameObject.SetActive(i < count);
            }

            _diceHolder.SetSocketCount(count);
        }

        public void InitDice() => InstantiateDice();

        private void HandleRollComplete()
        {
            _diceHolder.RepositionDice();
            OnRollCompleted?.Invoke();
        }

        private void HandleDiceToggle() => OnDiceToggled?.Invoke();

        private void Awake()
        {
            InstantiateDice();
            _diceHolder.Initialize(_dices);
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

        private void InstantiateDice()
        {
            ClearDice();

            RewardsData receivedRewards = GameProgress.GetReceivedRewards();
            int additionalDiceCount = receivedRewards.RewardTypes.Count(r => r == RewardType.AdditionalDice);
            int diceCount = _config.DiceStartCount + additionalDiceCount;

            for (int i = 0; i < diceCount; i++)
            {
                Dice dice = Instantiate(_dice, _diceSpawn);
                _dices.Add(dice);
            }
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

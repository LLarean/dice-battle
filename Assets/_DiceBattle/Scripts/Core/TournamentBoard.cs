using System;
using System.Collections.Generic;
using DiceBattle.Data;
using UnityEngine;

namespace DiceBattle.Core
{
    /// <summary>
    /// Single <see cref="DiceShaker"/> shared by two <see cref="DiceHolder"/>s
    /// (player / bot). Rolls run one side at a time - the static dice animation
    /// cannot handle parallel rolls.
    /// </summary>
    public class TournamentBoard : MonoBehaviour
    {
        private readonly List<Dice> _playerDices = new();
        private readonly List<Dice> _enemyDices = new();

        [SerializeField] private GameConfig _config;
        [Space]
        [SerializeField] private DiceShaker _diceShaker;
        [Space]
        [SerializeField] private DiceHolder _playerHolder;
        [SerializeField] private DiceHolder _enemyHolder;
        [Space]
        [SerializeField] private Dice _dicePrefab;
        [SerializeField] private Transform _playerSpawn;
        [SerializeField] private Transform _enemySpawn;

        private DiceHolder _activeHolder;

        public event Action OnRollCompleted;
        public event Action OnPlayerDiceToggled;

        public List<Dice> PlayerDices => _playerDices;
        public List<Dice> EnemyDices => _enemyDices;

        public bool HavePlayerSelectedDice => _playerHolder.Selected.Count > 0;
        public bool HavePlayerUnselectedDice => _playerHolder.Selected.Count < _playerDices.Count;

        public void RollPlayer()
        {
            _activeHolder = _playerHolder;
            _diceShaker.Roll(_playerHolder.Occupied);
        }

        public void RerollPlayerSelected()
        {
            _activeHolder = _playerHolder;
            _diceShaker.Roll(_playerHolder.Selected);
        }

        public void RollEnemy()
        {
            _activeHolder = _enemyHolder;
            _diceShaker.Roll(_enemyHolder.Occupied);
        }

        public void RerollEnemySelected()
        {
            _activeHolder = _enemyHolder;
            _diceShaker.Roll(_enemyHolder.Selected);
        }

        public void ResetPlayerDice() => _playerDices.ForEach(dice => dice.ResetToEmpty());

        public void ResetEnemyDice() => _enemyDices.ForEach(dice => dice.ResetToEmpty());

        public void EnablePlayerDice() => _playerDices.ForEach(dice => dice.EnableButton());

        public void DisablePlayerDice() => _playerDices.ForEach(dice => dice.DisableButton());

        public void ClearPlayerSelection() => _playerDices.ForEach(dice => dice.ClearSelection());

        public void SetPlayerSelectionStatus(bool isSelected) => _playerDices.ForEach(dice => dice.SetSelection(isSelected));

        public void SelectEnemyDice(IEnumerable<Dice> dice)
        {
            _enemyDices.ForEach(d => d.ClearSelection());
            foreach (Dice d in dice)
            {
                d.SetSelection(true);
            }
        }

        private void HandleRollComplete()
        {
            _activeHolder.AnimateDiceToSlots(() => OnRollCompleted?.Invoke());
        }

        private void HandlePlayerDiceToggle() => OnPlayerDiceToggled?.Invoke();

        private void Awake()
        {
            int count = _config.DiceStartCount;

            InstantiateDice(_playerSpawn, count, _playerDices, interactable: true);
            InstantiateDice(_enemySpawn, count, _enemyDices, interactable: false);

            _playerHolder.Initialize(_playerDices);
            _enemyHolder.Initialize(_enemyDices);
        }

        private void Start()
        {
            _diceShaker.OnRollCompleted += HandleRollComplete;
            _playerHolder.OnDiceToggled += HandlePlayerDiceToggle;
        }

        private void OnDestroy()
        {
            _diceShaker.OnRollCompleted -= HandleRollComplete;
            _playerHolder.OnDiceToggled -= HandlePlayerDiceToggle;
        }

        private void InstantiateDice(Transform spawn, int count, List<Dice> target, bool interactable)
        {
            for (int i = 0; i < count; i++)
            {
                Dice dice = Instantiate(_dicePrefab, spawn);
                if (interactable == false)
                {
                    dice.DisableButton();
                }

                target.Add(dice);
            }
        }
    }
}

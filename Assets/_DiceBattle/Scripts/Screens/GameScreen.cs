using System;
using System.Collections.Generic;
using System.Linq;
using DiceBattle.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DiceBattle.Screens
{
    public class GameScreen : MonoBehaviour
    {
        [SerializeField] private UnitPanel _player;
        [SerializeField] private UnitPanel _enemy;
        [SerializeField] private GameBoard _gameBoard;
        [Space]
        [SerializeField] private Button _context;
        [SerializeField] private TextMeshProUGUI _contextLabel;

        public event Action OnContextClicked;

        public List<Dice> Dices => _gameBoard.Dices;

        public void SetPlayerData(UnitData unitData) => _player.SetUnitData(unitData);

        public void SetEnemyData(UnitData unitData) => _enemy.SetUnitData(unitData);

        public void EnableDiceInteractable() => _gameBoard.EnableDiceInteractable();

        public void DisableDiceInteractable() => _gameBoard.DisableDiceInteractable();

        public void RollDice() => _gameBoard.RollDice();

        public void RerollSelectedDice() => _gameBoard.RerollSelectedDice();

        public void SetContextLabel(string label) => _contextLabel.text = label;

        public void UpdatePlayerHealth(int currentHealth) => _player.UpdateCurrentHealth(currentHealth);

        public void UpdatePlayerDefense(int defense) => _player.UpdateDefense(defense);

        public void ResetSelection() => _gameBoard.ClearAllSelection();

        public void UpdateEnemyDisplay() => _enemy.UpdateDisplay();

        #region Event Handlers

        private void ContextClick() => OnContextClicked?.Invoke();

        private void HandleDiceToggle()
        {
            SetContextLabel("Reroll Selected");

            bool isAllSelected = _gameBoard.Dices.All(dice => dice.IsSelected);
            bool isAllUnselected = _gameBoard.Dices.All(dice => !dice.IsSelected);

            if (isAllSelected)
            {
                SetContextLabel("Reroll All");
            }
            if (isAllUnselected)
            {
                SetContextLabel("Skip");
            }
        }

        private void HandleRollComplete()
        {
            //TODO Add lock/unlock action buttons
        }

        #endregion

        #region Unity lifecycle

        private void Start()
        {
            _context.onClick.AddListener(ContextClick);
            _gameBoard.OnDiceToggled += HandleDiceToggle;
            _gameBoard.OnRollCompleted += HandleRollComplete;

            SetContextLabel("Roll All");
        }

        private void OnDestroy()
        {
            _context.onClick.RemoveAllListeners();
            _gameBoard.OnDiceToggled -= HandleDiceToggle;
            _gameBoard.OnRollCompleted -= HandleRollComplete;
        }

        #endregion
    }
}

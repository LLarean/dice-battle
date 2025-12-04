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
        [Space]
        [SerializeField] private GameBoard _gameBoard;
        [Space]
        [SerializeField] private Button _context;
        [SerializeField] private TextMeshProUGUI _contextLabel;

        public event Action OnContextClicked;

        public List<Dice> Dices => _gameBoard.Dices;

        public void Initialize(int maxHealth)
        {
            _player.HideAllStats();
            _player.SetMaxHealth(maxHealth);

            SetContextLabel("Roll All");
        }

        public void EnableDiceInteractable() => _gameBoard.EnableAllDice();

        public void DisableDiceInteractable() => _gameBoard.DisableAllDice();

        public void RollAllDice() => _gameBoard.RollAllDice();

        public void RerollSelectedDice() => _gameBoard.RerollSelectedDice();

        public void SetContextLabel(string label) => _contextLabel.text = label;

        // public void ShowAttempts(int attemptsCount) => _dicePanel.ShowAttempts(attemptsCount);

        public void UpdateHealth(int currentHealth) => _player.UpdateHealth(currentHealth);

        public void UpdatePlayerDefense(int defense) => _player.UpdateDefense(defense);

        public void UnlockAll() => _gameBoard.ClearAllSelection();

        private void Start()
        {
            _context.onClick.AddListener(ContextClick);
            _gameBoard.OnDiceToggled += HandleDiceToggle;
            _gameBoard.OnRollCompleted += HandleRollComplete;
        }

        private void OnDestroy()
        {
            _context.onClick.RemoveAllListeners();
            _gameBoard.OnDiceToggled -= HandleDiceToggle;
            _gameBoard.OnRollCompleted -= HandleRollComplete;
        }

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
            //TODO logic unlock button
        }
    }
}

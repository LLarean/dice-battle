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
        [SerializeField] private DicePanel _dicePanel;
        [Space]
        [SerializeField] private Button _context;
        [SerializeField] private TextMeshProUGUI _contextLabel;

        public event Action OnContextClicked;
        
        public List<Dice> Dices => _dicePanel.Dices;

        public void Initialize(int maxHealth)
        {
            _player.HideAllStats();
            _player.SetMaxHealth(maxHealth);
            
            SetContextLabel("Roll All");
            _dicePanel.Initialize();
        }

        public void EnableDiceInteractable() => _dicePanel.EnableInteractable();
        
        public void DisableDiceInteractable() => _dicePanel.DisableInteractable();

        public void RollUnlockedDice() => _dicePanel.RollUnlockedDice();

        public void SetContextLabel(string label) => _contextLabel.text = label;

        public void ShowAttempts(int attemptsCount) => _dicePanel.ShowAttempts(attemptsCount);

        public void UpdateHealth(int currentHealth) => _player.UpdateHealth(currentHealth);

        public void UpdatePlayerDefense(int defense) => _player.UpdateDefense(defense);

        public void UnlockAll() => _dicePanel.UnlockAll();
        
        private void Start()
        {
            _context.onClick.AddListener(ContextClick);
            _dicePanel.OnDiceClicked += DiceClick;
        }

        private void OnDestroy()
        {
            _context.onClick.RemoveAllListeners();
            _dicePanel.OnDiceClicked -= DiceClick;
        }

        private void ContextClick() => OnContextClicked?.Invoke();

        private void DiceClick()
        {
            SetContextLabel("Roll Unselected");
            
            bool isAllLocked = _dicePanel.Dices.All(dice => dice.IsMarked);
            bool isAllUnlocked = _dicePanel.Dices.All(dice => !dice.IsMarked);

            if (isAllLocked)
            {
                SetContextLabel("End Turn");
            }
            if (isAllUnlocked)
            {
                SetContextLabel("Roll All");
            }
        }
    }
}
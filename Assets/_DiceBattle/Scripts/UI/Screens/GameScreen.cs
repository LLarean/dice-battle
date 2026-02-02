using System.Collections.Generic;
using System.Linq;
using DiceBattle.Audio;
using DiceBattle.Core;
using DiceBattle.Data;
using DiceBattle.Events;
using GameSignals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class GameScreen : Screen, IChangeHandler
    {
        [SerializeField] private GameConfig _config;
        [Space]
        [SerializeField] private ContextBackground _contextBackground;
        [SerializeField] private UnitPanel _player;
        [SerializeField] private UnitPanel _enemy;
        [SerializeField] private GameBoard _gameBoard;
        [Space]
        [SerializeField] private Button _context;
        [SerializeField] private TextMeshProUGUI _contextLabel;
        [SerializeField] private Button _all;

        private GameLogic _gameLogic;

        public List<Dice> Dices => _gameBoard.Dices;
        public bool HaveSelectedDice => _gameBoard.HaveSelectedDice;

        public void UpdateRewards() => _gameLogic.UpdateData();

        public void SetPlayerData(UnitData unitData) => _player.SetUnitData(unitData);

        public void UpdatePlayerStats() => _player.UpdateStats();

        public void SetEnemyData(UnitData unitData)
        {
            _contextBackground.SetSprite(unitData.Background);
            _enemy.SetUnitData(unitData);
        }

        public void SetContextLabel(string label) => _contextLabel.text = label;

        #region Damage/Healing Mediation

        public void PlayerTakeDamage(int damageAmount) => _player.TakeDamage(damageAmount);

        public void EnemyTakeDamage(int damageAmount) => _enemy.TakeDamage(damageAmount);

        public void PlayerTakeHeal(int healAmount) => _player.TakeHeal(healAmount);

        #endregion

        #region Dice

        public void SetDiceCount(int count) => _gameBoard.SetDiceCount(count);

        public void EnableDiceInteractable() => _gameBoard.EnableDiceInteractable();

        public void DisableDiceInteractable() => _gameBoard.DisableDiceInteractable();

        public void RollDice() => _gameBoard.RollDice();

        public void RerollSelectedDice() => _gameBoard.RerollSelectedDice();

        public void ResetSelection() => _gameBoard.ClearAllSelection();

        public void ToggleAllDice() => _gameBoard.ToggleAll();

        #endregion

        #region Damage/Healing Animation

        public void PlayerAnimateHeal() => _player.AnimateHeal();

        public void PlayerAnimateDamage() => _player.AnimateDamage();

        public void EnemyAnimateHeal() => _enemy.AnimateHeal();

        public void EnemyAnimateDamage() => _enemy.AnimateDamage();

        #endregion

        #region Event Handlers

        private void HandleContextClicked() => _gameLogic.ContextClick();

        private void HandleAllClicked() => _gameLogic.AllClick();

        private void HandleDiceToggle()
        {
            SetContextLabel("Перебросить выбранные"); // TODO Translation

            bool isAllSelected = _gameBoard.Dices.All(dice => dice.IsSelected);
            bool isAllUnselected = _gameBoard.Dices.All(dice => !dice.IsSelected);

            if (isAllSelected)
            {
                SetContextLabel("Перебросить все"); // TODO Translation
            }
            if (isAllUnselected)
            {
                SetContextLabel("Закончить"); // TODO Translation
            }
        }

        private void HandleRollComplete()
        {
            //TODO Add lock/unlock action buttons
        }

        #endregion

        #region Unity lifecycle

        private void Awake() => _gameLogic = new GameLogic(_config, this);

        private void Start()
        {
            _context.onClick.AddListener(HandleContextClicked);
            _all.onClick.AddListener(HandleAllClicked);
            _gameBoard.OnDiceToggled += HandleDiceToggle;
            _gameBoard.OnRollCompleted += HandleRollComplete;

            SetContextLabel("Бросить все"); // TODO Translation
        }

        private void OnDestroy()
        {
            _context.onClick.RemoveAllListeners();
            _gameBoard.OnDiceToggled -= HandleDiceToggle;
            _gameBoard.OnRollCompleted -= HandleRollComplete;
        }

        private void OnEnable()
        {
            _gameLogic.InitializeGame();
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlayMusic(SoundType.Battle));
        }

        #endregion
    }
}

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
    public class TournamentScreen : Screen
    {
        [SerializeField] private GameConfig _config;
        [Space]
        [SerializeField] private ContextBackground _contextBackground;
        [SerializeField] private Sprite _background;
        [SerializeField] private UnitPanel _player;
        [SerializeField] private UnitPanel _enemy;
        [SerializeField] private TournamentBoard _board;
        [SerializeField] private ShakeDetector _shakeDetector;
        [Space]
        [SerializeField] private Button _help;
        [SerializeField] private Button _context;
        [SerializeField] private TextMeshProUGUI _contextLabel;
        [SerializeField] private Button _all;
        [SerializeField] private RollButtonHint _rollButtonHint;

        private TournamentLogic _logic;

        public List<Dice> PlayerDices => _board.PlayerDices;
        public List<Dice> EnemyDices => _board.EnemyDices;
        public bool HavePlayerSelectedDice => _board.HavePlayerSelectedDice;
        public bool HavePlayerUnselectedDice => _board.HavePlayerUnselectedDice;

        #region Unit data

        public void SetPlayerData(UnitData unitData) => _player.SetUnitData(unitData);

        public void SetEnemyData(UnitData unitData)
        {
            if (_background != null)
            {
                _contextBackground.SetSprite(_background);
            }

            _enemy.SetUnitData(unitData);
        }

        public void SetContextLabel(string label) => _contextLabel.text = label;

        public void SetPlayerDicePreview(int armorBonus, int damageBonus, int healBonus) =>
            _player.SetDicePreview(armorBonus, damageBonus, healBonus);

        public void ClearPlayerDicePreview() => _player.ClearDicePreview();

        #endregion

        #region Damage / healing

        public void PlayerTakeDamage(int amount) => _player.TakeDamage(amount);

        public void EnemyTakeDamage(int amount) => _enemy.TakeDamage(amount);

        public void PlayerTakeHeal(int amount) => _player.TakeHeal(amount);

        public void EnemyTakeHeal(int amount) => _enemy.TakeHeal(amount);

        public void PlayerAnimateDamage() => _player.AnimateDamage();

        public void EnemyAnimateDamage() => _enemy.AnimateDamage();

        #endregion

        #region Dice

        public void ResetPlayerDice() => _board.ResetPlayerDice();

        public void ResetEnemyDice() => _board.ResetEnemyDice();

        public void EnablePlayerDice()
        {
            _board.EnablePlayerDice();
            _rollButtonHint.SetPaused(false);
        }

        public void DisablePlayerDice()
        {
            _board.DisablePlayerDice();
            _rollButtonHint.SetPaused(true);
        }

        public void RollPlayer() => _board.RollPlayer();

        public void RerollPlayerSelected() => _board.RerollPlayerSelected();

        public void RollEnemy() => _board.RollEnemy();

        public void RerollEnemySelected() => _board.RerollEnemySelected();

        public void SelectEnemyDice(IEnumerable<Dice> dice) => _board.SelectEnemyDice(dice);

        public void SetPlayerSelectionStatus(bool isSelected) => _board.SetPlayerSelectionStatus(isSelected);

        #endregion

        #region Event handlers

        private void HandleHelpClicked() =>
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.HelpWindow));

        private void HandleContextClicked()
        {
            _rollButtonHint.Notify();
            _logic.ContextClick();
        }

        private void HandleAllClicked()
        {
            _rollButtonHint.Notify();
            _logic.AllClick();
            HandleDiceToggle();
        }

        private void HandleDiceToggle()
        {
            bool allSelected = _board.PlayerDices.All(dice => dice.IsSelected);
            bool allUnselected = _board.PlayerDices.All(dice => !dice.IsSelected);

            if (allSelected)
            {
                SetContextLabel("Перебросить все"); // TODO Localization
            }
            else if (allUnselected)
            {
                SetContextLabel("Закончить"); // TODO Localization
            }
            else
            {
                SetContextLabel("Перебросить выбранные"); // TODO Localization
            }
        }

        private void HandleRollComplete() => _logic.OnRollCompleted();

        #endregion

        #region Unity lifecycle

        private void Awake() => _logic = new TournamentLogic(_config, this);

        private void Start()
        {
            _help.onClick.AddListener(HandleHelpClicked);
            _context.onClick.AddListener(HandleContextClicked);
            _all.onClick.AddListener(HandleAllClicked);
            _board.OnPlayerDiceToggled += HandleDiceToggle;
            _board.OnRollCompleted += HandleRollComplete;
            _shakeDetector.OnShake += HandleContextClicked;
        }

        private void OnDestroy()
        {
            _help.onClick.RemoveAllListeners();
            _context.onClick.RemoveAllListeners();
            _all.onClick.RemoveAllListeners();
            _board.OnPlayerDiceToggled -= HandleDiceToggle;
            _board.OnRollCompleted -= HandleRollComplete;
            _shakeDetector.OnShake -= HandleContextClicked;
        }

        private void OnEnable()
        {
            _logic.InitializeMatch();
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlayMusic(SoundType.Battle));
        }

        #endregion
    }
}

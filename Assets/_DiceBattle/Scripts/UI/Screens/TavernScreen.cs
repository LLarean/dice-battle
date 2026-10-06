using Assets.SimpleLocalization.Scripts;
using DiceBattle.Animations;
using DiceBattle.Audio;
using DiceBattle.Data;
using DiceBattle.Events;
using DiceBattle.Global;
using DiceBattle.Localization;
using GameSignals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class TavernScreen : Screen
    {
        [Space]
        [SerializeField] private Button _tournament;
        [SerializeField] private Button _restart;
        [SerializeField] private Button _start;
        [SerializeField] private TextMeshProUGUI _startLabel;
        [SerializeField] private Button _inventory;
        [SerializeField] private GameObject _newDiceBadge;
        [SerializeField] private TMP_Text _newDiceCount;
        [Space]
        [SerializeField] private Innkeeper _innkeeper;
        [SerializeField] private GameConfig _gameConfig;

        private const float _badgePopScale = 1.3f;
        private const float _badgePopDuration = 0.4f;
        private const float _badgePopDelay = 0.4f;

        private bool IsFullClear => GameData.CompletedLevels >= _gameConfig.Enemies.Count;

        #region Unity lifecycle

        private void Start()
        {
            _tournament.onClick.AddListener(HandleTournamentClick);
            _restart.onClick.AddListener(HandleRestartClick);
            _start.onClick.AddListener(HandleStartClick);
            _inventory.onClick.AddListener(HandleInventoryClick);
        }

        private void OnDestroy()
        {
            _tournament.onClick.RemoveAllListeners();
            _restart.onClick.RemoveAllListeners();
            _start.onClick.RemoveAllListeners();
            _inventory.onClick.RemoveAllListeners();
            LeanTween.cancel(_newDiceBadge);
        }

        private void OnEnable()
        {
            _innkeeper.ShowMessage(_gameConfig);
            SetLabel();
            ShowNewDiceBadge();
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlayMusic(SoundType.Tavern));

            // The tavern can already be open under the loot window, so OnEnable alone would miss the pick.
            GameData.OnNewDiceCountChanged += ShowNewDiceBadge;
        }

        private void OnDisable() => GameData.OnNewDiceCountChanged -= ShowNewDiceBadge;

        #endregion

        #region Handlers

        private void HandleTournamentClick()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.TournamentPyramidScreen));
        }

        private void HandleRestartClick()
        {
            string title = LocalizationManager.Localize(LocKeys.Window.RestartTitle);
            string message = LocalizationManager.Localize(LocKeys.Window.RestartMessage);
            string acceptText = LocalizationManager.Localize(LocKeys.Button.Again);
            string cancelText = LocalizationManager.Localize(LocKeys.Button.Stay);

            var confirmData = new ConfirmData(title,
                message,
                onAccept: () =>
                {
                    GameData.ResetAll();
                    DefaultInventory.InitializeDefault(_gameConfig.DiceStartCount);
                    SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.MainMenu));
                }, acceptText: acceptText, cancelText: cancelText);

            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.ConfirmWindow));
            SignalSystem.Raise<IConfirmHandler>(h => h.SetConfirmData(confirmData));
        }

        private void HandleStartClick()
        {
            if (IsFullClear)
            {
                GameData.AdvanceNewGamePlus();
                DefaultInventory.InitializeDefault(_gameConfig.DiceStartCount);
            }

            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.GameScreen));
        }

        private void HandleInventoryClick()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.InventoryWindow));
        }

        #endregion

        private void ShowNewDiceBadge()
        {
            int newDiceCount = GameData.NewDiceCount;

            LeanTween.cancel(_newDiceBadge);
            _newDiceBadge.transform.localScale = Vector3.one;
            _newDiceBadge.SetActive(newDiceCount > 0);
            _newDiceCount.text = newDiceCount.ToString();

            if (newDiceCount > 0)
            {
                DiceAnimation.Lift(_newDiceBadge, _badgePopScale, _badgePopDuration, _badgePopDelay);
            }
        }

        private void SetLabel()
        {
            _startLabel.text = IsFullClear
                ? LocalizationManager.Localize(LocKeys.Button.NewGame)
                : $"{LocalizationManager.Localize(LocKeys.Button.Level)} {GameData.CompletedLevels + 1}";
        }
    }
}

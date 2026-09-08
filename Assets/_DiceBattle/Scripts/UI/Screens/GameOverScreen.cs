using Assets.SimpleLocalization.Scripts;
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
    public class GameOverScreen : Screen
    {
        [Space]
        [SerializeField] private TextMeshProUGUI _finalScore;
        [SerializeField] private Button _restart;
        [SerializeField] private TextMeshProUGUI _restartLabel;
        [Space]
        [SerializeField] private GameConfig _config;

        private bool IsFullClear => GameData.CompletedLevels >= _config.Enemies.Count;

        private void Start() => _restart.onClick.AddListener(HandleRestartClick);

        private void OnDestroy() => _restart.onClick.RemoveAllListeners();

        private void OnEnable()
        {
            // TODO Localization
            _finalScore.text = $"Вы победили {GameData.CompletedLevels} врагов!";

            string key = IsFullClear ? LocKeys.Button.ToTavern : LocKeys.Button.Repeat;
            _restartLabel.text = LocalizationManager.Localize(key);
        }

        private void HandleRestartClick()
        {
            if (IsFullClear)
            {
                SignalSystem.Raise<IScreenHandler>(handler => handler.CloseTopWindow());
                SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.TavernScreen));
                return;
            }

            var confirmData = new ConfirmData(LocalizationManager.Localize(LocKeys.Window.RestartTitle),
                LocalizationManager.Localize(LocKeys.Window.RestartMessage),
                onAccept: () =>
                {
                    GameData.ResetAll();
                    SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.GameScreen));
                }, acceptText: LocalizationManager.Localize(LocKeys.Button.Repeat), cancelText: LocKeys.Button.Stay);

            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.ConfirmWindow));
            SignalSystem.Raise<IConfirmHandler>(h => h.SetConfirmData(confirmData));
        }
    }
}

using DiceBattle.Data;
using DiceBattle.Events;
using DiceBattle.Global;
using GameSignals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class GameOverScreen : Screen
    {
        [SerializeField] private GameConfig _config;
        [SerializeField] private TextMeshProUGUI _finalScore;
        [SerializeField] private Button _restart;
        [SerializeField] private TextMeshProUGUI _restartLabel;

        private bool IsFullClear => GameData.CompletedLevels >= _config.Enemies.Count;

        private void Start() => _restart.onClick.AddListener(HandleRestartClick);

        private void OnDestroy() => _restart.onClick.RemoveAllListeners();

        private void OnEnable()
        {
            _finalScore.text = $"Вы победили {GameData.CompletedLevels} врагов!"; // TODO Translation
            _restartLabel.text = IsFullClear ? "В таверну" : "Заново"; // TODO Translation
        }

        private void HandleRestartClick()
        {
            if (IsFullClear)
            {
                SignalSystem.Raise<IScreenHandler>(handler => handler.CloseTopWindow());
                SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.TavernScreen));
                return;
            }

            var confirmData = new ConfirmData("Похоронить героев?",
                "Герои сдаются, но всегда приходят новые. Весь прогресс и собранная коллекция кубиков будут потеряны безвозвратно.",
                onAccept: () =>
                {
                    GameData.ResetAll();
                    SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.GameScreen));
                }, acceptText: "Заново", cancelText: "Остаться");

            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.ConfirmWindow));
            SignalSystem.Raise<IConfirmHandler>(h => h.SetConfirmData(confirmData));
        }
    }
}

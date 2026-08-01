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
        [SerializeField] private TextMeshProUGUI _finalScore;
        [SerializeField] private Button _restart;

        private void Start() => _restart.onClick.AddListener(HandleRestartClick);

        private void OnDestroy() => _restart.onClick.RemoveAllListeners();

        private void OnEnable()
        {
            _finalScore.text = $"Вы победили {GameData.CompletedLevels} врагов!"; // TODO Translation
        }

        private void HandleRestartClick()
        {
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

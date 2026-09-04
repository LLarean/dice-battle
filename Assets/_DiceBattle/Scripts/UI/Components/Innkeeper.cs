using Assets.SimpleLocalization.Scripts;
using DiceBattle.Events;
using DiceBattle.Localization;
using GameSignals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DiceBattle.UI
{
    public class Innkeeper : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private Button _quest;

        public void ShowMessage()
        {
            AnimateIn();

            int count = LocalizationManager.CountIndexedKeys(LocKeys.Innkeeper.MessagePrefix);
            string key = $"{LocKeys.Innkeeper.MessagePrefix}[{Random.Range(0, count)}]";
            _message.text = LocalizationManager.Localize(key);
        }

        private void Start()
        {
            _quest.onClick.AddListener(HandleQuestClicked);
        }

        private void OnDestroy()
        {
            _quest.onClick.RemoveAllListeners();
        }

        private void HandleQuestClicked()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.QuestWindow));
        }

        private void AnimateIn()
        {
            // TODO Add animation
        }

    }
}

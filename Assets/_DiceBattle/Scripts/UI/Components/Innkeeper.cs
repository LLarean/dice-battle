using System;
using DiceBattle.Data;
using DiceBattle.Events;
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
        [Space]
        [SerializeField] private InnkeeperConfig _config;

        public void ShowMessage()
        {
            AnimateIn();

            // TODO Add translation to other languages
            int randomCount = Random.Range(0, _config.Messages.Count);
            _message.text =  _config.Messages[randomCount];
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

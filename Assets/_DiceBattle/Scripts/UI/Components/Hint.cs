using System;
using DiceBattle.Events;
using GameSignals;
using TMPro;
using UnityEngine;

namespace DiceBattle.UI
{
    // Subscribes before any screen's OnEnable, so a battle restored mid-turn can show its hint at once.
    [DefaultExecutionOrder(-1)]
    public class Hint : MonoBehaviour, IHintHandler
    {
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private bool _isVisibleOnStart;

        // public void ShowAttempts(int attemptCount)
        // {
        //     gameObject.SetActive(true);
        //     _message.text = $"There are {attemptCount} attempts left";
        // }
        //
        // public void ShowRoll()
        // {
        //     _message.text = "Click the Roll button!";
        // }
        //
        // public void Show()
        // {
        //      _message.text = "You can choose the dice to avoid throwing them";
        // }

        public void Show(string message)
        {
            _message.text = message;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            SignalSystem.Subscribe(this);
            gameObject.SetActive(_isVisibleOnStart);
        }

        private void OnDestroy()
        {
            SignalSystem.Unsubscribe(this);
        }
    }
}

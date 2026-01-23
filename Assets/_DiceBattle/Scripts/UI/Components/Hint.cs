using System;
using DiceBattle.Events;
using GameSignals;
using TMPro;
using UnityEngine;

namespace DiceBattle.UI
{
    public class Hint : MonoBehaviour, IHintHandler
    {
        [SerializeField] private TextMeshProUGUI _message;

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

        private void Start()
        {
            SignalSystem.Subscribe(this);
        }

        private void OnDestroy()
        {
            SignalSystem.Unsubscribe(this);
        }
    }
}

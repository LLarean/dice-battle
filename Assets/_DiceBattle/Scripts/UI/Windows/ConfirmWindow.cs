using System;
using DiceBattle.Events;
using GameSignals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class ConfirmWindow : Screen, IConfirmHandler
    {
        [Space]
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _message;
        [Space]
        [SerializeField] private Button _accept;
        [SerializeField] private Button _cancel;

        private Action _onAccept;
        private Action _onCancel;

        private void Awake() => SignalSystem.Subscribe(this);

        private void Start()
        {
            _accept.onClick.AddListener(HandleAccept);
            _cancel.onClick.AddListener(HandleCancel);
        }

        private void OnDestroy()
        {
            SignalSystem.Unsubscribe(this);
            _accept.onClick.RemoveAllListeners();
            _cancel.onClick.RemoveAllListeners();
        }

        public void SetConfirmData(ConfirmData data)
        {
            _title.text = data.Title;
            _message.text = data.Message;
            _onAccept = data.OnAccept;
            _onCancel = data.OnCancel;
        }

        private void HandleAccept()
        {
            Hide();
            _onAccept?.Invoke();
        }

        private void HandleCancel()
        {
            Hide();
            _onCancel?.Invoke();
        }
    }
}

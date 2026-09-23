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
        [SerializeField] private TextMeshProUGUI _acceptText;
        [SerializeField] private Button _cancel;
        [SerializeField] private TextMeshProUGUI _cancelText;

        private Action _onAccept;
        private Action _onCancel;
        private bool _isAnswered;
        private float _answerAllowedFrom;

        public void SetConfirmData(ConfirmData data)
        {
            _title.text = data.Title;
            _message.text = data.Message;
            _onAccept = data.OnAccept;
            _onCancel = data.OnCancel;
            _isAnswered = false;

            if (string.IsNullOrEmpty(data.AcceptText) == false)
            {
                _acceptText.text = data.AcceptText;
            }

            if (string.IsNullOrEmpty(data.CancelText) == false)
            {
                _cancelText.text = data.CancelText;
            }
        }

        #region Unity lifecycle

        private void Awake() => SignalSystem.Subscribe(this);

        // Until the window lock expires CloseTopWindow is ignored, so an early answer would leave the window open.
        private void OnEnable() => _answerAllowedFrom = Time.unscaledTime + ScreenChanger.TransitionLockDuration;

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

        #endregion

        #region Handlers

        private void HandleAccept()
        {
            if (TryAnswer() == false)
            {
                return;
            }

            SignalSystem.Raise<IScreenHandler>(handler => handler.CloseTopWindow());
            _onAccept?.Invoke();
        }

        private void HandleCancel()
        {
            if (TryAnswer() == false)
            {
                return;
            }

            SignalSystem.Raise<IScreenHandler>(handler => handler.CloseTopWindow());
            _onCancel?.Invoke();
        }

        #endregion

        private bool TryAnswer()
        {
            if (_isAnswered || Time.unscaledTime < _answerAllowedFrom)
            {
                return false;
            }

            _isAnswered = true;
            return true;
        }
    }
}

using System;

namespace DiceBattle.Events
{
    public readonly struct ConfirmData
    {
        public readonly string Title;
        public readonly string Message;
        public readonly Action OnAccept;
        public readonly Action OnCancel;

        public ConfirmData(string title, string message, Action onAccept, Action onCancel = null)
        {
            Title = title;
            Message = message;
            OnAccept = onAccept;
            OnCancel = onCancel;
        }
    }
}

using GameSignals;

namespace DiceBattle.Events
{
    public interface IConfirmHandler : IGlobalSubscriber
    {
        void SetConfirmData(ConfirmData data);
    }
}

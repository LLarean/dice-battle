using GameSignals;

namespace DiceBattle.Events
{
    public interface IHintHandler : IGlobalSubscriber
    {
        void Show(string message);
        void Hide();
    }
}

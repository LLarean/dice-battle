using GameSignals;

namespace DiceBattle.Events
{
    public interface ITopBarHandler : IGlobalSubscriber
    {
        void Show();
        void Hide();
    }
}

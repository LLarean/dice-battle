using GameSignals;

namespace DiceBattle.Audio
{
    public interface ITopBarHandler : IGlobalSubscriber
    {
        void Show();
        void Hide();
    }
}

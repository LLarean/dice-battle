using GameSignals;

namespace DiceBattle.Audio
{
    public interface IOptionsWindowHandler : IGlobalSubscriber
    {
        void Show();
        void Hide();
    }
}

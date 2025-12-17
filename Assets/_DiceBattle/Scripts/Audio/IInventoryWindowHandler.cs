using GameSignals;

namespace DiceBattle.Audio
{
    public interface IInventoryWindowHandler : IGlobalSubscriber
    {
        void Show();
        void Hide();
    }
}

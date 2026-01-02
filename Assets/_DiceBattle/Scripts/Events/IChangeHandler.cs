using GameSignals;

namespace DiceBattle.Events
{
    public interface IChangeHandler : IGlobalSubscriber
    {
        void UpdateRewards();
    }
}

using DiceBattle.Core;
using GameSignals;

namespace DiceBattle.Events
{
    public interface IDiceResultHandler : IGlobalSubscriber
    {
        void OnDiceLanded(DiceHolder source, Dice dice, int amount);
    }
}

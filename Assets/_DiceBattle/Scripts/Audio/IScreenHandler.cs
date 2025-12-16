using GameSignals;

namespace DiceBattle.Audio
{
    public interface IScreenHandler : IGlobalSubscriber
    {
        void ShowScreen(ScreenType screenType);
        void Back();
    }
}

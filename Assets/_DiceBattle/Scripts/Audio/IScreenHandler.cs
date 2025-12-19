using GameSignals;

namespace DiceBattle.Audio
{
    public interface IScreenHandler : IGlobalSubscriber
    {
        void ShowScreen(ScreenType screenType);
        void ShowWindow(ScreenType screenType);
        void Back();
    }
}

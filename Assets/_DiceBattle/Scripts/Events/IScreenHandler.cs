using DiceBattle.UI;
using GameSignals;

namespace DiceBattle.Events
{
    public interface IScreenHandler : IGlobalSubscriber
    {
        void ShowScreen(ScreenType screenType);
        void ShowWindow(ScreenType screenType);
        void CloseTopWindow();
        void Back();
    }
}

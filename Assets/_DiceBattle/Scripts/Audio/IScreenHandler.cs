using GameSignals;

namespace DiceBattle.Audio
{
    public interface IScreenHandler : IGlobalSubscriber
    {
        void ShowScreen(ScreenType screenType);
    }

    public interface ITopBatHandler : IGlobalSubscriber
    {
        void Show();
        void Hide();
    }
}

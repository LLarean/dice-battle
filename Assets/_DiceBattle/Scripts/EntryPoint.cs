using DiceBattle.Events;
using DiceBattle.UI;
using GameSignals;
using UnityEngine;

namespace DiceBattle
{
    public class EntryPoint : MonoBehaviour
    {
        private void Start()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.MainMenu));
        }
    }
}

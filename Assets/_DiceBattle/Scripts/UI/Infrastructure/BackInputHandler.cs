using DiceBattle.Events;
using GameSignals;
using UnityEngine;

namespace DiceBattle.UI
{
    public class BackInputHandler : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SignalSystem.Raise<IScreenHandler>(handler => handler.Back());
            }
        }
    }
}

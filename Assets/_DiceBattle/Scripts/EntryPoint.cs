using System;
using DiceBattle.Events;
using DiceBattle.UI;
using GameSignals;
using UnityEngine;

namespace DiceBattle
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private DefaultInventory _defaultInventory;

        private void Start()
        {
            Application.targetFrameRate = 60;
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.MainMenu));

            _defaultInventory.SetDefaultInventory();
            _defaultInventory.SetEquippedItems();
        }
    }
}

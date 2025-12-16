using System;
using System.Collections.Generic;
using DiceBattle.Audio;
using GameSignals;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle
{
    public class DungeonsScreen : MonoBehaviour
    {
        [SerializeField] private List<LevelItem> _levelItems;
        [SerializeField] private Button _back;

        private void Start()
        {
            foreach (LevelItem levelItem in _levelItems)
            {
                levelItem.OnClicked += OpenLevel;
            }

            _back.onClick.AddListener(ShowMainMenu);
        }

        private void OnDestroy()
        {
            foreach (LevelItem levelItem in _levelItems)
            {
                levelItem.OnClicked -= OpenLevel;
            }

            _back.onClick.RemoveAllListeners();
        }

        private void OnEnable()
        {
            SignalSystem.Raise<ITopBarHandler>(handler => handler.Hide());
        }

        private void ShowMainMenu()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.MainMenu));
        }

        private void OpenLevel(int levelIndex)
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.GameScreen));
        }
    }
}

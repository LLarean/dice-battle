using System;
using System.Collections.Generic;
using DiceBattle.Animations;
using DiceBattle.Audio;
using DiceBattle.Core;
using DiceBattle.Data;
using DiceBattle.Events;
using DiceBattle.Global;
using GameSignals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class MainMenuScreen : Screen
    {
        [Space]
        [SerializeField] private RectTransform _rootUI;
        [Space]
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Button _options;
        [SerializeField] private Button _start;
        [SerializeField] private GameConfig _config;
        [Space]
        [SerializeField] private List<Dice> _dice;
        [SerializeField] private RectTransform _rollAnimationArea;
        [SerializeField] private RectTransform _bottomButtons;

        private GameObjectAnimations _gameObjectAnimations;
        private readonly List<Action> _diceToggleHandlers = new();

        private void Awake()
        {
            _gameObjectAnimations = new GameObjectAnimations(_rootUI);
            _gameObjectAnimations.SetParams(.2f, .5f, LeanTweenType.easeOutBack);
        }

        private void Start()
        {
            _options.onClick.AddListener(HandleOptionsClick);
            _start.onClick.AddListener(HandleStartClick);

            foreach (Dice dice in _dice)
            {
                Dice clickedDice = dice;
                Action handler = () => HandleDiceToggled(clickedDice);

                _diceToggleHandlers.Add(handler);
                clickedDice.OnToggled += handler;
            }
        }

        private void OnDestroy()
        {
            _options.onClick.RemoveAllListeners();
            _start.onClick.RemoveAllListeners();

            for (int i = 0; i < _dice.Count; i++)
            {
                _dice[i].OnToggled -= _diceToggleHandlers[i];
            }

            LeanTween.cancel(gameObject);
        }

        private void OnEnable()
        {
            _gameObjectAnimations.SlideIn(_title.rectTransform);
            _gameObjectAnimations.SlideIn(_bottomButtons, -1);
            DiceAnimation.Animate(_dice, _rollAnimationArea);

            SignalSystem.Raise<ITopBarHandler>(handler => handler.Hide());
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlayMusic(SoundType.Menu));
        }

        private void HandleOptionsClick()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.OptionsWindow));
        }

        private void HandleStartClick()
        {
            ScreenType targetScreen = _config.CanSaveBattle && BattleSaveData.HasSavedBattle()
                ? ScreenType.GameScreen
                : ScreenType.TavernScreen;

            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(targetScreen));
            SignalSystem.Raise<ITopBarHandler>(handler => handler.Show());
        }

        private void HandleDiceToggled(Dice dice)
        {
            dice.Roll();

            CheckEasterEgg();
        }

        private void CheckEasterEgg()
        {
            DiceValue firstValue = _dice[0].DiceValue;

            for (int i = 1; i < _dice.Count; i++)
            {
                if (_dice[i].DiceValue != firstValue)
                {
                    return;
                }
            }

            TriggerEasterEgg(firstValue);
        }

        private void TriggerEasterEgg(DiceValue diceValue)
        {
            Debug.Log($"Easter egg triggered! All dice show {diceValue}.");
        }
    }
}

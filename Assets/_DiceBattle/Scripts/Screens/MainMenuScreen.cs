using System.Collections.Generic;
using DiceBattle.Audio;
using DiceBattle.Core;
using GameSignals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.Screens
{
    public class MainMenuScreen : MonoBehaviour
    {
        [SerializeField] private RectTransform _rootUI;
        [Space]
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private List<Dice> _dice;
        [Space]
        [SerializeField] private Button _options;
        [SerializeField] private Button _start;
        [Space]
        [SerializeField] private RectTransform _rollAnimationArea;
        [SerializeField] private RectTransform _bottomButtons;

        private GameObjectAnimations _gameObjectAnimations;
        private DiceAnimation _diceAnimation;

        private void Awake()
        {
            _gameObjectAnimations = new GameObjectAnimations(_rootUI);
            _gameObjectAnimations.SetParams(.2f, .5f, LeanTweenType.easeOutBack);
            _diceAnimation = new DiceAnimation(_rollAnimationArea);
        }

        private void Start()
        {
            _options.onClick.AddListener(HandleOptionsClick);
            _start.onClick.AddListener(HandleStartClick);
        }

        private void OnDestroy()
        {
            _options.onClick.RemoveAllListeners();
            _start.onClick.RemoveAllListeners();
        }

        private void OnEnable()
        {
            _gameObjectAnimations.SlideIn(_title.rectTransform);
            _gameObjectAnimations.SlideIn(_bottomButtons, -1);

            _diceAnimation.Animate(_dice);

            SignalSystem.Raise<ITopBarHandler>(handler => handler.Hide());
        }

        private void HandleStartClick()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.DungeonsScreen));
            SignalSystem.Raise<ITopBarHandler>(handler => handler.Show());
        }

        private void HandleOptionsClick()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.OptionsWindow));
        }
    }
}

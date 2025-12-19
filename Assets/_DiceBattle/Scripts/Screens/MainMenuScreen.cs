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
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private List<Dice> _dice;
        [Space]
        [SerializeField] private Button _options;
        [SerializeField] private Button _start;
        [Space]
        [SerializeField] private RectTransform _rollAnimationArea;
        [SerializeField] private RectTransform _bottomButtons;

        private RectTransform _canvasRect;

        private void Awake() => _canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

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
            var gameObjectAnimations = new GameObjectAnimations(_canvasRect);
            gameObjectAnimations.SetParams(.2f, .5f, LeanTweenType.easeOutBack);

            gameObjectAnimations.SlideIn(_title.rectTransform);
            gameObjectAnimations.SlideIn(_bottomButtons, -1);

            new DiceAnimation(_rollAnimationArea).Animate(_dice);

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

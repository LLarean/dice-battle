using System;
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
        [SerializeField] private Button _restart;
        [SerializeField] private Button _start;
        [Space]
        [SerializeField] private DiceRollAnimation _diceRollAnimation;

        private RectTransform _canvasRect;

        private void Awake() => _canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        private void Start()
        {
            _options.onClick.AddListener(HandleOptionsClick);
            _start.onClick.AddListener(HandleStartClick);
            _start.onClick.AddListener(HandleStartClick);
        }

        private void OnDestroy() => _start.onClick.RemoveAllListeners();

        private void OnEnable()
        {
            SlideIn(_title.rectTransform);
            SlideIn(_start.GetComponent<RectTransform>(), -1);

            _diceRollAnimation.RollDice(_dice);
        }

        private void HandleStartClick()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.GameScreen));
        }

        private void HandleOptionsClick()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
        }


        private void SlideIn(RectTransform animationObject, int direction = 1)
        {
            Vector2 startPosition = Vector2.zero;

            if (LeanTween.isTweening(animationObject.gameObject))
            {
                LeanTween.cancel(animationObject.gameObject);
            }
            else
            {
                startPosition = animationObject.GetComponent<RectTransform>().anchoredPosition;
            }

            float canvasHeight = _canvasRect.rect.height;

            Vector2 offScreenPos = startPosition + new Vector2(0, canvasHeight * direction);
            animationObject.anchoredPosition = offScreenPos;

            LeanTween.move(animationObject, startPosition, 1)
                .setDelay(.5f)
                .setEase(LeanTweenType.easeOutBack);
        }
    }
}

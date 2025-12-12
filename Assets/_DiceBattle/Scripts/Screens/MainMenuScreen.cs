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
        [SerializeField] private Button _start;
        [Space]
        [SerializeField] private DiceRollAnimation _diceRollAnimation;

        private void Start() => _start.onClick.AddListener(HandleStartClick);

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

            RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            float canvasHeight = canvasRect.rect.height;

            Vector2 offScreenPos = startPosition + new Vector2(0, canvasHeight * direction);
            animationObject.anchoredPosition = offScreenPos;

            LeanTween.move(animationObject, startPosition, 1)
                .setDelay(.5f)
                .setEase(LeanTweenType.easeOutBack);
        }
    }
}

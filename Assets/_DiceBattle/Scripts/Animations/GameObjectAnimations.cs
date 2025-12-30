using System;
using UnityEngine;

namespace DiceBattle.Animations
{
    public class GameObjectAnimations
    {
        private readonly RectTransform _canvasRect;

        private float _time = 1;
        private float _delay = .5f;
        private LeanTweenType _leanTweenType = LeanTweenType.easeOutBack;

        public event Action OnAnimationComplete;

        public GameObjectAnimations(RectTransform canvasRect)
        {
            _canvasRect = canvasRect;
        }

        public void SetParams(float time, float delay, LeanTweenType leanTweenType)
        {
            _time = time;
            _delay = delay;
            _leanTweenType = leanTweenType;
        }

        public void SlideIn(RectTransform animationObject, int direction = 1)
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

            LeanTween.move(animationObject, startPosition, _time)
                .setDelay(_delay)
                .setEase(_leanTweenType)
                .setOnComplete(() =>
                {
                    OnAnimationComplete?.Invoke();
                });
        }
    }
}

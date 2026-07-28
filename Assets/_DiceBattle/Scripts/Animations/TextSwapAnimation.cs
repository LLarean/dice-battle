using System;
using TMPro;
using UnityEngine;

namespace DiceBattle.Animations
{
    public static class TextSwapAnimation
    {
        private const float _outDuration = 0.12f;
        private const float _inDuration = 0.22f;

        public static void AnimateSwap(TMP_Text text, Action onSwap)
        {
            LeanTween.cancel(text.gameObject);
            text.alpha = 1f;

            LeanTween.value(text.gameObject, text.alpha, 0f, _outDuration)
                .setEase(LeanTweenType.easeInQuad)
                .setOnUpdate(a => text.alpha = a)
                .setOnComplete(() =>
                {
                    onSwap?.Invoke();

                    LeanTween.value(text.gameObject, text.alpha, 1f, _inDuration)
                        .setEase(LeanTweenType.easeOutQuad)
                        .setOnUpdate(a => text.alpha = a);
                });
        }
    }
}

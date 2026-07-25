using System;
using UnityEngine;

namespace DiceBattle.Animations
{
    public static class PortraitSwapAnimation
    {
        private const float _outDuration = 0.12f;
        private const float _inDuration = 0.22f;
        private const float _tiltAngle = 12f;

        public static void AnimateSwap(RectTransform rect, int direction, Action onSwap)
        {
            LeanTween.cancel(rect.gameObject);

            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;

            LeanTween.scaleX(rect.gameObject, 0.05f, _outDuration)
                .setEase(LeanTweenType.easeInBack)
                .setOnComplete(() =>
                {
                    onSwap?.Invoke();

                    rect.localRotation = Quaternion.Euler(0f, 0f, _tiltAngle * direction);

                    LeanTween.scaleX(rect.gameObject, 1f, _inDuration).setEase(LeanTweenType.easeOutBack);
                    LeanTween.rotateZ(rect.gameObject, 0f, _inDuration).setEase(LeanTweenType.easeOutBack);
                });

            LeanTween.rotateZ(rect.gameObject, -_tiltAngle * direction, _outDuration).setEase(LeanTweenType.easeInBack);
        }
    }
}

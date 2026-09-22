using System;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DiceBattle.Animations
{
    public static class FloatingTextAnimation
    {
        private const float _appearDuration = 0.15f;
        private const float _appearStartScale = 0.3f;

        private const float _flyDelay = 0.1f;
        private const float _flyDuration = 0.3f;
        private const float _flyEndScale = 0.6f;

        private const float _riseDistance = 90f;
        private const float _riseDuration = 0.9f;
        private const float _fadeDuration = 0.3f;

        public static void FlyTo(TMP_Text label, Transform target, Action onArrived)
        {
            GameObject go = label.gameObject;
            Appear(go, 1f);

            LeanTween.move(go, target.position, _flyDuration)
                .setDelay(_flyDelay)
                .setEase(LeanTweenType.easeInQuad);

            LeanTween.scale(go, Vector3.one * _flyEndScale, _flyDuration)
                .setDelay(_flyDelay)
                .setEase(LeanTweenType.easeInQuad)
                .setOnComplete(() =>
                {
                    Object.Destroy(go);
                    onArrived?.Invoke();
                });
        }

        public static void Rise(TMP_Text label, float popScale = 1.3f)
        {
            GameObject go = label.gameObject;
            Appear(go, popScale);

            LeanTween.moveLocalY(go, go.transform.localPosition.y + _riseDistance, _riseDuration)
                .setEase(LeanTweenType.easeOutQuad);

            LeanTween.value(go, 1f, 0f, _fadeDuration)
                .setDelay(_riseDuration - _fadeDuration)
                .setOnUpdate(alpha => label.alpha = alpha)
                .setOnComplete(() => Object.Destroy(go));
        }

        private static void Appear(GameObject go, float popScale)
        {
            go.transform.localScale = Vector3.one * _appearStartScale;

            LeanTween.scale(go, Vector3.one * popScale, _appearDuration)
                .setEase(LeanTweenType.easeOutBack)
                .setOnComplete(() =>
                {
                    if (popScale > 1f)
                    {
                        LeanTween.scale(go, Vector3.one, _appearDuration).setEase(LeanTweenType.easeOutQuad);
                    }
                });
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.Animations
{
    public static class HealthAnimation
    {
        private const float _flashDuration = 0.1f;
        private const int _flashCount = 3;

        private const float _healPulseScale = 1.08f;
        private const float _healPulseDuration = 0.25f;

        private const float _damageShakeOffset = 6f;
        private const float _damageShakeStep = 0.04f;
        private const float _damageSquash = 0.9f;
        private const float _damageSquashDuration = 0.08f;

        public static void AnimateHeal(Image portrait)
        {
            LeanTween.cancel(portrait.gameObject);

            RectTransform rect = portrait.rectTransform;
            rect.localScale = Vector3.one;

            // Slow, gentle "breathe" pulse - contrasts with the sharp damage shake.
            LeanTween.scale(rect, Vector3.one * _healPulseScale, _healPulseDuration)
                .setEase(LeanTweenType.easeInOutSine)
                .setOnComplete(() => LeanTween.scale(rect, Vector3.one, _healPulseDuration).setEase(LeanTweenType.easeInOutSine));

            LTSeq sequence = LeanTween.sequence();

            for (int i = 0; i < _flashCount; i++)
            {
                sequence.append(LeanTween.value(portrait.gameObject, portrait.color, Color.green, _flashDuration)
                    .setOnUpdate(val => portrait.color = val));

                sequence.append(LeanTween.value(portrait.gameObject, Color.green, Color.white, _flashDuration)
                    .setOnUpdate(val => portrait.color = val));
            }

            sequence.append(() => portrait.color = Color.white);
        }

        public static void AnimateDamage(Image portrait)
        {
            LeanTween.cancel(portrait.gameObject);

            RectTransform rect = portrait.rectTransform;
            rect.localScale = Vector3.one;

            float baseX = rect.anchoredPosition.x;

            // Sharp, decaying shake plus a quick squash - reads as an impact, not just a color flash.
            LTSeq shake = LeanTween.sequence();
            shake.append(LeanTween.moveX(rect, baseX - _damageShakeOffset, _damageShakeStep).setEase(LeanTweenType.easeOutQuad));
            shake.append(LeanTween.moveX(rect, baseX + _damageShakeOffset, _damageShakeStep).setEase(LeanTweenType.easeInOutSine));
            shake.append(LeanTween.moveX(rect, baseX - _damageShakeOffset * 0.6f, _damageShakeStep).setEase(LeanTweenType.easeInOutSine));
            shake.append(LeanTween.moveX(rect, baseX, _damageShakeStep).setEase(LeanTweenType.easeInOutSine));

            LeanTween.scale(rect, Vector3.one * _damageSquash, _damageSquashDuration)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnComplete(() => LeanTween.scale(rect, Vector3.one, _damageSquashDuration).setEase(LeanTweenType.easeOutBack));

            LTSeq sequence = LeanTween.sequence();

            for (int i = 0; i < _flashCount; i++)
            {
                sequence.append(LeanTween.value(portrait.gameObject, portrait.color, Color.red, _flashDuration)
                    .setOnUpdate(val => portrait.color = val));

                sequence.append(LeanTween.value(portrait.gameObject, Color.red, Color.white, _flashDuration)
                    .setOnUpdate(val => portrait.color = val));
            }

            sequence.append(() => portrait.color = Color.white);
        }
    }
}

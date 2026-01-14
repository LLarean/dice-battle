using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.Animations
{
    public static class HealthAnimation
    {
        private const float _flashDuration = 0.1f;
        private const int _flashCount = 3;

        public static void AnimateHeal(Image portrait)
        {
            LeanTween.cancel(portrait.gameObject);

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

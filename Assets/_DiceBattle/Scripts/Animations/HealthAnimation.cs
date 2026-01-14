using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.Animations
{
    public class HealthAnimation
    {
        private readonly Image _portrait;

        private const float _flashDuration = 0.1f;
        private const int _flashCount = 3;

        public HealthAnimation(Image portrait)
        {
            _portrait = portrait;
        }

        public void AnimateHeal()
        {
            LeanTween.cancel(_portrait.gameObject);

            LTSeq sequence = LeanTween.sequence();

            for (int i = 0; i < _flashCount; i++)
            {
                sequence.append(LeanTween.value(_portrait.gameObject, _portrait.color, Color.green, _flashDuration)
                    .setOnUpdate(val => _portrait.color = val));

                sequence.append(LeanTween.value(_portrait.gameObject, Color.green, Color.white, _flashDuration)
                    .setOnUpdate(val => _portrait.color = val));
            }

            sequence.append(() => _portrait.color = Color.white);
        }

        public void AnimateDamage()
        {
            LeanTween.cancel(_portrait.gameObject);

            LTSeq sequence = LeanTween.sequence();

            for (int i = 0; i < _flashCount; i++)
            {
                sequence.append(LeanTween.value(_portrait.gameObject, _portrait.color, Color.red, _flashDuration)
                    .setOnUpdate(val => _portrait.color = val));

                sequence.append(LeanTween.value(_portrait.gameObject, Color.red, Color.white, _flashDuration)
                    .setOnUpdate(val => _portrait.color = val));
            }

            sequence.append(() => _portrait.color = Color.white);
        }
    }
}

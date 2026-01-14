using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.Animations
{
    public class HealthAnimation
    {
        private readonly Image _portrait;
        private readonly Slider _health;

        private const float _flashDuration = 0.1f;
        private const int _flashCount = 3;

        public HealthAnimation(Image portrait, Slider health)
        {
            _portrait = portrait;
            _health = health;
        }

        public void Animate(int currentHealth)
        {
            Color flashColor = currentHealth > _health.value ? Color.green : Color.red;

            LeanTween.cancel(_portrait.gameObject);

            LTSeq sequence = LeanTween.sequence();

            for (int i = 0; i < _flashCount; i++)
            {
                sequence.append(LeanTween.value(_portrait.gameObject, _portrait.color, flashColor, _flashDuration)
                    .setOnUpdate(val => _portrait.color = val));

                sequence.append(LeanTween.value(_portrait.gameObject, flashColor, Color.white, _flashDuration)
                    .setOnUpdate(val => _portrait.color = val));
            }

            sequence.append(() => _portrait.color = Color.white);
        }
    }
}

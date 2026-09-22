using UnityEngine;

namespace DiceBattle.Animations
{
    public static class ShakeAnimation
    {
        private const float _strength = 18f;
        private const float _duration = 0.4f;

        public static void Shake(RectTransform rect)
        {
            Vector3 basePosition = rect.localPosition;

            LeanTween.value(rect.gameObject, 1f, 0f, _duration)
                .setOnUpdate(power => rect.localPosition = basePosition + (Vector3)(Random.insideUnitCircle * _strength * power))
                .setOnComplete(() => rect.localPosition = basePosition);
        }
    }
}

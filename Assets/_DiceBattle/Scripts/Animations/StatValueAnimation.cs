using UnityEngine;

namespace DiceBattle.Animations
{
    public static class StatValueAnimation
    {
        private const float _punchScale = 1.3f;
        private const float _duration = 0.2f;

        public static void AnimatePunch(Transform target)
        {
            LeanTween.cancel(target.gameObject);
            target.localScale = Vector3.one;

            LeanTween.scale(target.gameObject, Vector3.one * _punchScale, _duration * 0.5f)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnComplete(() =>
                {
                    LeanTween.scale(target.gameObject, Vector3.one, _duration * 0.5f)
                        .setEase(LeanTweenType.easeInQuad);
                });
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

namespace DiceBattle
{
    public class ButtonScaleAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private const float _duration = 0.2f;
        private const float _pressScale = 0.9f;

        private Vector3 _originalScale;

        public void OnPointerDown(PointerEventData eventData)
        {
            LeanTween.cancel(gameObject);
            LeanTween.scale(gameObject, _originalScale * _pressScale, _duration)
                .setEase(LeanTweenType.easeOutQuad);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            LeanTween.cancel(gameObject);
            LeanTween.scale(gameObject, _originalScale, _duration)
                .setEase(LeanTweenType.easeOutQuad);
        }

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        private void OnDestroy()
        {
            LeanTween.cancel(gameObject);
        }
    }
}

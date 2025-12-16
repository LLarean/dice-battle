using DiceBattle.Audio;
using GameSignals;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DiceBattle
{
    public class BaseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private const float _duration = 0.2f;
        private const float _pressScale = 0.9f;

        [SerializeField] private Button _button;
        [SerializeField] private SoundType _soundType = SoundType.Click;

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

        private void Awake() => _originalScale = transform.localScale;

        private void Start() => _button.onClick.AddListener(HandleClick);

        private void HandleClick() => SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(_soundType));

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
            LeanTween.cancel(gameObject);
        }
    }
}

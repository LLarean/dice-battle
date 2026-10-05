using DiceBattle.Audio;
using DiceBattle.Events;
using GameSignals;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class BaseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private const float _duration = 0.2f;
        private const float _pressScale = 0.9f;

        [SerializeField] private Button _button;
        [SerializeField] private SoundType _soundType = SoundType.Click;

        private Vector3 _originalScale;
        private int _tweenId = -1;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_button.interactable == false)
            {
                return;
            }

            ScaleTo(_originalScale * _pressScale);
        }

        public void OnPointerUp(PointerEventData eventData) => ScaleTo(_originalScale);

        // Cancels only its own tween: the object may be mid-animation (a rolling die), and cancelling that would drop its completion callback.
        private void ScaleTo(Vector3 scale)
        {
            LeanTween.cancel(_tweenId);
            _tweenId = LeanTween.scale(gameObject, scale, _duration)
                .setEase(LeanTweenType.easeOutQuad)
                .id;
        }

        private void Awake() => _originalScale = transform.localScale;

        private void Start() => _button.onClick.AddListener(HandleClick);

        private void HandleClick()
        {
            if (_button.interactable == false)
            {
                return;
            }

            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(_soundType));
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
            LeanTween.cancel(_tweenId);
        }
    }
}

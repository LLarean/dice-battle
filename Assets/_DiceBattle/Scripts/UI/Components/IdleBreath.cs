using UnityEngine;

namespace DiceBattle.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class IdleBreath : MonoBehaviour
    {
        [SerializeField] private float _scaleAmount = 0.05f;
        [SerializeField] private float _duration = 1.6f;
        [SerializeField] private float _startDelay = 0f;

        private Vector3 _originalScale;
        private int _tweenId = -1;

        private void Awake() => _originalScale = transform.localScale;

        private void OnEnable()
        {
            transform.localScale = _originalScale;

            _tweenId = LeanTween.scale(gameObject, _originalScale * (1f + _scaleAmount), _duration)
                .setDelay(_startDelay)
                .setEase(LeanTweenType.easeInOutSine)
                .setLoopPingPong(-1)
                .id;
        }

        private void OnDisable()
        {
            if (_tweenId != -1)
            {
                LeanTween.cancel(_tweenId);
                _tweenId = -1;
            }

            transform.localScale = _originalScale;
        }
    }
}

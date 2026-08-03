using DiceBattle.Global;
using UnityEngine;

namespace DiceBattle.UI
{
    public class RollButtonHint : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [Space]
        [SerializeField] private float _idleThreshold = 10f;
        [SerializeField] private float _pulseMinAlpha = 0.5f;
        [SerializeField] private float _pulseMaxAlpha = 1f;
        [SerializeField] private float _pulseDuration = 0.6f;

        private float _idleTimer;
        private bool _isPulsing;
        private bool _isPaused;

        public void Notify()
        {
            _idleTimer = 0f;
            StopPulse();
        }

        public void SetPaused(bool isPaused)
        {
            _isPaused = isPaused;

            if (isPaused)
            {
                StopPulse();
            }
            else
            {
                _idleTimer = 0f;
            }
        }

        private void Update()
        {
            if (_isPaused || _isPulsing)
            {
                return;
            }

            _idleTimer += Time.unscaledDeltaTime;

            if (_idleTimer >= _idleThreshold)
            {
                StartPulse();
            }
        }

        private void OnEnable()
        {
            _idleTimer = GameData.HasEverRolledDice ? 0f : _idleThreshold;
        }

        private void OnDisable() => StopPulse();

        private void StartPulse()
        {
            _isPulsing = true;

            _canvasGroup.alpha = _pulseMinAlpha;
            LeanTween.alphaCanvas(_canvasGroup, _pulseMaxAlpha, _pulseDuration)
                .setEase(LeanTweenType.easeInOutSine)
                .setLoopPingPong(-1);
        }

        private void StopPulse()
        {
            if (!_isPulsing)
            {
                return;
            }

            _isPulsing = false;
            _idleTimer = 0f;

            LeanTween.cancel(_canvasGroup.gameObject);
            _canvasGroup.alpha = 1f;
        }
    }
}

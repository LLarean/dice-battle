using UnityEngine;

namespace DiceBattle.Animations
{
    [DisallowMultipleComponent]
    public class BackgroundParallax : MonoBehaviour
    {
        [SerializeField] private Vector2 _maxOffset = new(40f, 25f);
        [SerializeField] private float _smooth = 6f;
        [SerializeField] private bool _invert = true;

        // Gyroscope tilt range in degrees mapped to ±1
        [SerializeField] private float _gyroTiltRange = 20f;
        // How fast the current way of holding the phone becomes the new centre
        [SerializeField] private float _gyroRecenterSpeed = 0.5f;

        private RectTransform _rect;
        private Vector2 _basePosition;
        private bool _useGyro;
        private Gyroscope _gyro;
        private Vector2 _gyroCenter;
        private bool _hasGyroCenter;

        private void Awake()
        {
            _rect = (RectTransform)transform;
            _basePosition = _rect.anchoredPosition;

            _useGyro = SystemInfo.supportsGyroscope && Application.isMobilePlatform;
            if (_useGyro)
            {
                _gyro = Input.gyro;
                _gyro.enabled = true;
            }
        }

        private void OnDisable()
        {
            _rect.anchoredPosition = _basePosition;
            _hasGyroCenter = false;
        }

        private void Update()
        {
            Vector2 normalized = _useGyro ? GetGyroNormalized() : GetMouseNormalized();
            normalized = Vector2.ClampMagnitude(normalized, 1f);

            if (_invert)
                normalized = -normalized;

            Vector2 target = _basePosition + Vector2.Scale(normalized, _maxOffset);
            _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, target, Time.deltaTime * _smooth);
        }

        private Vector2 GetMouseNormalized()
        {
            return new Vector2(
                (Input.mousePosition.x / Screen.width) * 2f - 1f,
                (Input.mousePosition.y / Screen.height) * 2f - 1f);
        }

        private Vector2 GetGyroNormalized()
        {
            // gravity vector in device space: x = roll, y = pitch
            Vector2 gravity = _gyro.gravity;

            // Tilt is measured from how the phone is held, not from lying flat: a hand-held phone is always pitched far past the range.
            if (_hasGyroCenter == false)
            {
                _gyroCenter = gravity;
                _hasGyroCenter = true;
            }

            _gyroCenter = Vector2.Lerp(_gyroCenter, gravity, Time.deltaTime * _gyroRecenterSpeed);

            Vector2 tilt = (gravity - _gyroCenter) / Mathf.Sin(_gyroTiltRange * Mathf.Deg2Rad);
            return new Vector2(Mathf.Clamp(tilt.x, -1f, 1f), Mathf.Clamp(tilt.y, -1f, 1f));
        }
    }
}
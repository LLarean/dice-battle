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

        private RectTransform _rect;
        private Vector2 _basePosition;
        private bool _useGyro;
        private Gyroscope _gyro;

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
            Vector3 gravity = _gyro.gravity;
            return new Vector2(
                Mathf.Clamp(gravity.x / Mathf.Sin(_gyroTiltRange * Mathf.Deg2Rad), -1f, 1f),
                Mathf.Clamp(gravity.y / Mathf.Sin(_gyroTiltRange * Mathf.Deg2Rad), -1f, 1f));
        }
    }
}
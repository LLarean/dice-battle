using UnityEngine;

namespace DiceBattle.Animations
{
    [DisallowMultipleComponent]
    public class BackgroundMouseParallax : MonoBehaviour
    {
        [SerializeField] private Vector2 _maxOffset = new(40f, 25f);
        [SerializeField] private float _smooth = 6f;
        [SerializeField] private bool _invert = true;

        private RectTransform _rect;
        private Vector2 _basePosition;

        private void Awake()
        {
            _rect = (RectTransform)transform;
            _basePosition = _rect.anchoredPosition;
        }

        private void OnDisable()
        {
            _rect.anchoredPosition = _basePosition;
        }

        private void Update()
        {
            var normalized = new Vector2(
                (Input.mousePosition.x / Screen.width) * 2f - 1f,
                (Input.mousePosition.y / Screen.height) * 2f - 1f);

            normalized = Vector2.ClampMagnitude(normalized, 1f);

            if (_invert)
            {
                normalized = -normalized;
            }

            Vector2 target = _basePosition + Vector2.Scale(normalized, _maxOffset);
            _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, target, Time.deltaTime * _smooth);
        }
    }
}

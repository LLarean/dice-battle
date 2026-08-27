using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SparkleField : MonoBehaviour
    {
        private static Sprite _sharedSprite;

        [SerializeField] private int _sparkleCount = 5;
        [SerializeField] private float _minInterval = 0.6f;
        [SerializeField] private float _maxInterval = 2f;
        [SerializeField] private float _minSize = 12f;
        [SerializeField] private float _maxSize = 24f;
        [SerializeField] private float _lifetime = 0.7f;
        [SerializeField] private Color _color = Color.white;

        private RectTransform[] _sparkles;
        private float[] _timers;

        private void Awake()
        {
            _sparkles = new RectTransform[_sparkleCount];
            _timers = new float[_sparkleCount];

            for (int i = 0; i < _sparkleCount; i++)
            {
                var go = new GameObject("Sparkle", typeof(RectTransform), typeof(Image));
                go.transform.SetParent(transform, false);

                var image = go.GetComponent<Image>();
                image.sprite = GetSharedSprite();
                image.color = _color;
                image.raycastTarget = false;

                var rect = (RectTransform)go.transform;
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);

                _sparkles[i] = rect;
                go.SetActive(false);

                _timers[i] = Random.Range(0f, _maxInterval);
            }
        }

        private void OnEnable()
        {
            for (int i = 0; i < _sparkleCount; i++)
            {
                _timers[i] = Random.Range(0f, _maxInterval);
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < _sparkleCount; i++)
            {
                LeanTween.cancel(_sparkles[i].gameObject);
                _sparkles[i].gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            for (int i = 0; i < _sparkleCount; i++)
            {
                if (_sparkles[i].gameObject.activeSelf)
                {
                    continue;
                }

                _timers[i] -= Time.unscaledDeltaTime;

                if (_timers[i] <= 0f)
                {
                    _timers[i] = Random.Range(_minInterval, _maxInterval);
                    Spawn(_sparkles[i]);
                }
            }
        }

        private void Spawn(RectTransform sparkle)
        {
            var area = (RectTransform)transform;
            float halfWidth = area.rect.width * 0.5f;
            float halfHeight = area.rect.height * 0.5f;

            sparkle.anchoredPosition = new Vector2(
                Random.Range(-halfWidth, halfWidth),
                Random.Range(-halfHeight, halfHeight));

            float size = Random.Range(_minSize, _maxSize);
            sparkle.sizeDelta = new Vector2(size, size);
            sparkle.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            sparkle.localScale = Vector3.zero;

            var go = sparkle.gameObject;
            go.SetActive(true);

            LeanTween.scale(go, Vector3.one, _lifetime * 0.4f)
                .setEase(LeanTweenType.easeOutBack);
            LeanTween.scale(go, Vector3.zero, _lifetime * 0.6f)
                .setDelay(_lifetime * 0.4f)
                .setEase(LeanTweenType.easeInQuad)
                .setOnComplete(() => go.SetActive(false));
        }

        private static Sprite GetSharedSprite()
        {
            if (_sharedSprite != null)
            {
                return _sharedSprite;
            }

            const int size = 32;
            const float center = size * 0.5f;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x + 0.5f - center) / center;
                    float dy = (y + 0.5f - center) / center;

                    float angle = Mathf.Atan2(dy, dx);
                    float radius = Mathf.Sqrt(dx * dx + dy * dy);

                    float lobe = Mathf.Abs(Mathf.Cos(angle * 2f));
                    float shape = Mathf.Pow(lobe, 6f) * 0.85f + 0.15f;
                    float alpha = Mathf.Clamp01((shape - radius) / shape);
                    alpha = Mathf.Pow(alpha, 1.5f);

                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.Apply();

            _sharedSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));

            return _sharedSprite;
        }
    }
}

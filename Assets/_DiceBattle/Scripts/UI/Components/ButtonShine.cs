using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class ButtonShine : MonoBehaviour
    {
        private static Sprite _sharedSprite;

        [SerializeField] private float _interval = 4f;
        [SerializeField] private float _duration = 0.6f;
        [SerializeField] private float _width = 60f;
        [SerializeField] [Range(0f, 1f)] private float _opacity = 0.5f;

        private RectTransform _streakRect;
        private float _timer;

        private void Awake()
        {
            if (gameObject.GetComponent<Mask>() == null)
            {
                var mask = gameObject.AddComponent<Mask>();
                mask.showMaskGraphic = true;
            }

            var streakGo = new GameObject("Shine", typeof(RectTransform), typeof(Image));
            streakGo.transform.SetParent(transform, false);

            _streakRect = (RectTransform)streakGo.transform;
            _streakRect.anchorMin = new Vector2(0f, 0f);
            _streakRect.anchorMax = new Vector2(0f, 1f);
            _streakRect.pivot = new Vector2(0.5f, 0.5f);
            _streakRect.sizeDelta = new Vector2(_width, 0f);

            var image = streakGo.GetComponent<Image>();
            image.sprite = GetSharedSprite();
            image.type = Image.Type.Sliced;
            image.raycastTarget = false;
            image.color = new Color(1f, 1f, 1f, _opacity);

            ResetPosition();
        }

        private void OnEnable()
        {
            _timer = _interval;
        }

        private void OnDisable()
        {
            LeanTween.cancel(_streakRect.gameObject);
        }

        private void Update()
        {
            _timer += Time.unscaledDeltaTime;

            if (_timer >= _interval)
            {
                _timer = 0f;
                PlaySweep();
            }
        }

        private void PlaySweep()
        {
            var parentRect = (RectTransform)transform;
            float travel = parentRect.rect.width + _width;

            ResetPosition();
            LeanTween.moveLocalX(_streakRect.gameObject, travel, _duration)
                .setEase(LeanTweenType.easeInOutSine);
        }

        private void ResetPosition()
        {
            var parentRect = (RectTransform)transform;
            _streakRect.localPosition = new Vector3(-parentRect.rect.width, 0f, 0f);
        }

        private static Sprite GetSharedSprite()
        {
            if (_sharedSprite != null)
            {
                return _sharedSprite;
            }

            const int size = 32;
            var texture = new Texture2D(size, 1, TextureFormat.RGBA32, false);
            texture.wrapMode = TextureWrapMode.Clamp;

            for (int x = 0; x < size; x++)
            {
                float t = x / (size - 1f);
                float alpha = 1f - Mathf.Abs(t - 0.5f) * 2f;
                texture.SetPixel(x, 0, new Color(1f, 1f, 1f, alpha));
            }

            texture.Apply();

            _sharedSprite = Sprite.Create(texture, new Rect(0, 0, size, 1), new Vector2(0.5f, 0.5f), 100f, 0,
                SpriteMeshType.FullRect, new Vector4(size / 2f - 1f, 0f, size / 2f - 1f, 0f));

            return _sharedSprite;
        }
    }
}

using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public enum TransitionKind
    {
        Window,
        Screen
    }

    public class Screen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TransitionKind _transitionKind = TransitionKind.Screen;
        [SerializeField] [ShowIf(nameof(_transitionKind), TransitionKind.Window)] [AllowNesting] private RectTransform _windowContent;
        [SerializeField] [ShowIf(nameof(_transitionKind), TransitionKind.Window)] [AllowNesting] private Image _dimmer;

        private RectTransform _rect;
        private float _dimmerTargetAlpha;
        private bool _initialized;

        private void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _rect = _windowContent != null ? _windowContent : (RectTransform)transform;

            if (_dimmer != null)
            {
                _dimmerTargetAlpha = _dimmer.color.a;
            }

            _initialized = true;
        }

        public void Show()
        {
            Initialize();
            LeanTween.cancel(gameObject);

            bool isWindow = _transitionKind == TransitionKind.Window;
            float startScale = isWindow ? 0.9f : 1.1f;
            float duration = isWindow ? 0.25f : 0.35f;

            _canvasGroup.alpha = 1f;
            _rect.localScale = Vector3.one * startScale;
            gameObject.SetActive(true);

            if (!isWindow)
            {
                _canvasGroup.alpha = 0f;
                LeanTween.alphaCanvas(_canvasGroup, 1f, duration).setEase(LeanTweenType.easeOutQuad);
            }

            LeanTween.scale(_rect, Vector3.one, duration).setEase(LeanTweenType.easeOutQuad);

            if (isWindow && _dimmer != null)
            {
                _dimmer.color = new Color(_dimmer.color.r, _dimmer.color.g, _dimmer.color.b, 0f);
                LeanTween.alpha(_dimmer.rectTransform, _dimmerTargetAlpha, duration).setEase(LeanTweenType.easeOutQuad).setRecursive(false);
            }
        }

        public void Hide()
        {
            Initialize();
            LeanTween.cancel(gameObject);

            bool isWindow = _transitionKind == TransitionKind.Window;
            float endScale = isWindow ? 0.9f : 1.1f;
            float duration = isWindow ? 0.2f : 0.3f;

            if (isWindow)
            {
                LeanTween.scale(_rect, Vector3.one * endScale, duration).setEase(LeanTweenType.easeInQuad)
                    .setOnComplete(() => { gameObject.SetActive(false); });
            }
            else
            {
                LeanTween.alphaCanvas(_canvasGroup, 0f, duration).setEase(LeanTweenType.easeInQuad)
                    .setOnComplete(() => { gameObject.SetActive(false); });
                LeanTween.scale(_rect, Vector3.one * endScale, duration).setEase(LeanTweenType.easeInQuad);
            }

            if (isWindow && _dimmer != null)
            {
                LeanTween.alpha(_dimmer.rectTransform, 0f, duration).setEase(LeanTweenType.easeInQuad).setRecursive(false);
            }
        }
    }
}

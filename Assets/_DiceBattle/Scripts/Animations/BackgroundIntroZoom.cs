using UnityEngine;

namespace DiceBattle.Animations
{
    [DisallowMultipleComponent]
    public class BackgroundIntroZoom : MonoBehaviour
    {
        [SerializeField] private float _startScale = 1.2f;
        [SerializeField] private float _targetScale = 1f;
        [SerializeField] private float _duration = 1.5f;
        [SerializeField] private LeanTweenType _ease = LeanTweenType.easeOutCubic;
        [SerializeField] private bool _playOnEnable = true;

        private RectTransform _rect;

        private void Awake()
        {
            _rect = (RectTransform)transform;
        }

        private void OnEnable()
        {
            if (_playOnEnable)
            {
                Play();
            }
        }

        private void Play()
        {
            LeanTween.cancel(gameObject);
            _rect.localScale = Vector3.one * _startScale;
            LeanTween.scale(_rect, Vector3.one * _targetScale, _duration).setEase(_ease);
        }
    }
}

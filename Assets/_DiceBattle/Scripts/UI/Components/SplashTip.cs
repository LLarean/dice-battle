using DiceBattle.Data;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DiceBattle.UI
{
    public class SplashTip : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private SplashTipsConfig _config;
        [Space]
        [SerializeField] private float _pulseMinAlpha = 0.4f;
        [SerializeField] private float _pulseMaxAlpha = 1f;
        [SerializeField] private float _pulseDuration = 0.8f;

        public void ShowRandomTip()
        {
            int randomIndex = Random.Range(0, _config.Tips.Count);
            _text.text = _config.Tips[randomIndex];

            Color faded = _text.color;
            faded.a = _pulseMinAlpha;
            _text.color = faded;

            LeanTween.alpha(_text.rectTransform, _pulseMaxAlpha, _pulseDuration)
                .setEase(LeanTweenType.easeInOutSine)
                .setLoopPingPong(-1);
        }
    }
}

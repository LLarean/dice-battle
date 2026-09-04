using Assets.SimpleLocalization.Scripts;
using DiceBattle.Localization;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DiceBattle.UI
{
    public class SplashTip : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [Space]
        [SerializeField] private float _pulseMinAlpha = 0.4f;
        [SerializeField] private float _pulseMaxAlpha = 1f;
        [SerializeField] private float _pulseDuration = 0.8f;

        public void ShowRandomTip()
        {
            int count = LocalizationManager.CountIndexedKeys(LocKeys.SplashTips.MessagePrefix);
            string key = $"{LocKeys.SplashTips.MessagePrefix}[{Random.Range(0, count)}]";
            _text.text = LocalizationManager.Localize(key);

            Color faded = _text.color;
            faded.a = _pulseMinAlpha;
            _text.color = faded;

            LeanTween.alpha(_text.rectTransform, _pulseMaxAlpha, _pulseDuration)
                .setEase(LeanTweenType.easeInOutSine)
                .setLoopPingPong(-1);
        }
    }
}

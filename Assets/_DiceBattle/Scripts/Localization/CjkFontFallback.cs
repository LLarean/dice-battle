using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;

namespace DiceBattle.Localization
{
    public class CjkFontFallback : MonoBehaviour
    {
        [SerializeField] private TMP_FontAsset _mainFont;
        [SerializeField] private TMP_FontAsset _japaneseFont;
        [SerializeField] private TMP_FontAsset _chineseFont;

        private void Awake()
        {
            Apply();
            LocalizationManager.OnLocalizationChanged += Apply;
        }

        private void OnDestroy()
        {
            LocalizationManager.OnLocalizationChanged -= Apply;
        }

        // Japanese and Chinese share code points but draw them differently,
        // so only the font of the current language may be a fallback.
        private void Apply()
        {
            TMP_FontAsset fallback = LocalizationManager.Language switch
            {
                nameof(SystemLanguage.Japanese) => _japaneseFont,
                nameof(SystemLanguage.Chinese) => _chineseFont,
                _ => null
            };

            _mainFont.fallbackFontAssetTable.Clear();

            if (fallback != null)
            {
                _mainFont.fallbackFontAssetTable.Add(fallback);
            }

            // The main font caches characters it found in a fallback; rereading drops that cache.
            _mainFont.ReadFontAssetDefinition();
        }
    }
}

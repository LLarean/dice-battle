using Assets.SimpleLocalization.Scripts;
using DiceBattle.Global;
using UnityEngine;

namespace DiceBattle.Localization
{
    public static class LocalizationInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitLocalizationOnLoad()
        {
            SystemLanguage language = GameSettings.HasSelectedLanguage
                ? GameSettings.SelectedLanguage
                : Application.systemLanguage;

            SetLanguage(language);
        }

        public static void SetLanguage(SystemLanguage language)
        {
            LocalizationManager.Read();

            language = MergeChineseVariants(language);

            SystemLanguage resolvedLanguage = AvailableLanguages.IsAvailable(language)
                ? language
                : AvailableLanguages.DefaultLanguage;

            LocalizationManager.Language = resolvedLanguage.ToString();
            GameSettings.SelectedLanguage = resolvedLanguage;
        }

        // Devices report the script variant, the game has a single Chinese translation.
        private static SystemLanguage MergeChineseVariants(SystemLanguage language) =>
            language is SystemLanguage.ChineseSimplified or SystemLanguage.ChineseTraditional
                ? SystemLanguage.Chinese
                : language;
    }
}

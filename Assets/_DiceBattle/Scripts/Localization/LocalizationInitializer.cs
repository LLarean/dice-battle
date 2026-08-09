using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts;
using UnityEngine;

namespace DiceBattle.Localization
{
    public static class LocalizationInitializer
    {
        private const SystemLanguage DefaultLanguage = SystemLanguage.English;

        private static readonly List<SystemLanguage> AvailableLanguages = new()
        {
            SystemLanguage.Russian,
            SystemLanguage.English,
            SystemLanguage.German,
            SystemLanguage.French,
            SystemLanguage.Portuguese,
            SystemLanguage.Japanese,
            SystemLanguage.Chinese,
            SystemLanguage.Spanish,
        };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitLocalization()
        {
            LocalizationManager.Read();
            SystemLanguage selectedLanguage = Application.systemLanguage;

            bool isAvailable = AvailableLanguages.Contains(selectedLanguage);
            LocalizationManager.Language = (isAvailable ? selectedLanguage : DefaultLanguage).ToString();

        }
    }
}

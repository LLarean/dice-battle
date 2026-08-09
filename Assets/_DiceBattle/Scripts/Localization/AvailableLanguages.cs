using System.Collections.Generic;
using UnityEngine;

namespace DiceBattle.Localization
{
    public static class AvailableLanguages
    {
        public const SystemLanguage DefaultLanguage = SystemLanguage.English;

        private static readonly List<SystemLanguage> Languages = new()
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

        public static bool IsAvailable(SystemLanguage language) => Languages.Contains(language);

        public static SystemLanguage GetNextLanguage(SystemLanguage current)
        {
            int index = Languages.IndexOf(current);
            int nextIndex = (index + 1) % Languages.Count;

            return Languages[nextIndex];
        }
    }
}

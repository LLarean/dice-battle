using UnityEngine;

namespace DiceBattle.Global
{
    public static class GameSettings
    {
        private const float DefaultMusicVolume = .5f;

        public static float MusicVolume => PlayerPrefs.GetFloat(PlayerPrefsKeys.MusicVolume, DefaultMusicVolume);
        public static float SoundVolume => PlayerPrefs.GetFloat(PlayerPrefsKeys.SoundVolume, DefaultMusicVolume);

        public static bool HasSelectedLanguage => PlayerPrefs.HasKey(PlayerPrefsKeys.SelectedLanguage);

        public static SystemLanguage SelectedLanguage
        {
            get => (SystemLanguage)PlayerPrefs.GetInt(PlayerPrefsKeys.SelectedLanguage, (int)SystemLanguage.English);
            set => PlayerPrefs.SetInt(PlayerPrefsKeys.SelectedLanguage, (int)value);
        }

        public static void ResetVolume()
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.MusicVolume, DefaultMusicVolume);
            PlayerPrefs.SetFloat(PlayerPrefsKeys.SoundVolume, DefaultMusicVolume);
        }

        public static void SetMusicVolume(float volume) => PlayerPrefs.SetFloat(PlayerPrefsKeys.MusicVolume, volume);

        public static void SetSoundVolume(float volume) => PlayerPrefs.SetFloat(PlayerPrefsKeys.SoundVolume, volume);
    }
}

using UnityEngine;

namespace DiceBattle.Global
{
    public static class GameSettings
    {
        public static float MusicVolume => PlayerPrefs.GetFloat(PlayerPrefsKeys.MusicVolume, 1);
        public static float SoundVolume => PlayerPrefs.GetFloat(PlayerPrefsKeys.SoundVolume, 1);

        public static void ResetVolume()
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.MusicVolume, 1);
            PlayerPrefs.SetFloat(PlayerPrefsKeys.SoundVolume, 1);
        }

        public static void SetMusicVolume(float volume) => PlayerPrefs.SetFloat(PlayerPrefsKeys.MusicVolume, volume);

        public static void SetSoundVolume(float volume) => PlayerPrefs.SetFloat(PlayerPrefsKeys.SoundVolume, volume);
    }
}

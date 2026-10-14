using DiceBattle.Core;
using UnityEngine;

namespace DiceBattle.Global
{
    public static class SaveVersion
    {
        // Raise when a change makes older saves wrong (enum values removed or reordered, a format change).
        // Progress from an older version is wiped on the next launch; settings and innkeeper memory stay.
        private const int _current = 1;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Check()
        {
            if (PlayerPrefs.GetInt(PlayerPrefsKeys.SaveVersion, 0) >= _current)
            {
                return;
            }

            if (PlayerPrefs.HasKey(PlayerPrefsKeys.AllItems))
            {
                Debug.LogWarning("Save is older than version " + _current + ", progress is wiped");
                GameData.ResetAll();
                TournamentBracket.Clear();
            }

            PlayerPrefs.SetInt(PlayerPrefsKeys.SaveVersion, _current);
            PlayerPrefs.Save();
        }
    }
}

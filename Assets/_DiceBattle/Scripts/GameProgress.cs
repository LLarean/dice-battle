using UnityEngine;

namespace DiceBattle
{
    public static class GameProgress
    {
        private static string _currentLevelKey = "CurrentLevel";

        public static int CurrentLevel => PlayerPrefs.GetInt(_currentLevelKey, 0);

        public static void IncrementCurrentLevel() => PlayerPrefs.SetInt(_currentLevelKey, CurrentLevel + 1);

        public static void ResetCurrentLevel() => PlayerPrefs.SetInt(_currentLevelKey, 0);

        public static RewardType GetRewardTypes()
        {
            return RewardType.DisableEmptyState;
        }
    }
}

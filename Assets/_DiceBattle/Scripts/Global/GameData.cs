using UnityEngine;

namespace DiceBattle.Global
{
    public static class GameData
    {
        private const string _playerPrefsKey = PlayerPrefsKeys.GameData;

        public static void Save(GameModel gameModel)
        {
            PlayerPrefs.SetString(_playerPrefsKey, JsonUtility.ToJson(gameModel));
            PlayerPrefs.Save();
        }

        public static GameModel Load()
        {
            string json = PlayerPrefs.GetString(_playerPrefsKey, null);

            if (string.IsNullOrEmpty(json))
            {
                return CreateNewRewardsData();
            }

            GameModel data = JsonUtility.FromJson<GameModel>(json);

            if (data == null)
            {
                Debug.LogWarning("Failed to deserialize game data, creating new");
                return CreateNewRewardsData();
            }

            return data;
        }

        public static void Clear() => PlayerPrefs.DeleteKey(_playerPrefsKey);

        private static GameModel CreateNewRewardsData()
        {
            return new GameModel();
        }
    }
}

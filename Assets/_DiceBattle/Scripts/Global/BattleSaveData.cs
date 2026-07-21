using System;
using DiceBattle.Core;
using UnityEngine;

namespace DiceBattle.Global
{
    [Serializable]
    public class UnitSnapshot
    {
        public string Name;
        public int MaxHealth;
        public int CurrentHealth;
        public int Damage;
        public int Armor;
    }

    [Serializable]
    public class BattleSnapshot
    {
        public UnitSnapshot PlayerState;
        public UnitSnapshot EnemyState;

        public int MaxDiceRerolls;
        public int RemainingDiceRerolls;
        public bool LastStandUsed;
    }

    public static class BattleSaveData
    {
        private const string _playerPrefsKey = PlayerPrefsKeys.BattleState;

        public static bool HasSavedBattle() => PlayerPrefs.HasKey(_playerPrefsKey);

        public static void Save(MatchData matchData)
        {
            var snapshot = new BattleSnapshot
            {
                PlayerState = ToSnapshot(matchData.PlayerData),
                EnemyState = ToSnapshot(matchData.EnemyData),
                MaxDiceRerolls = matchData.MaxDiceRerolls,
                RemainingDiceRerolls = matchData.RemainingDiceRerolls,
                LastStandUsed = matchData.LastStandUsed,
            };

            string json = JsonUtility.ToJson(snapshot);
            PlayerPrefs.SetString(_playerPrefsKey, json);
            PlayerPrefs.Save();
        }

        public static BattleSnapshot Load()
        {
            string json = PlayerPrefs.GetString(_playerPrefsKey, null);

            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            BattleSnapshot data = JsonUtility.FromJson<BattleSnapshot>(json);

            if (data == null)
            {
                Debug.LogWarning("Failed to deserialize battle save data");
                return null;
            }

            return data;
        }

        public static void Clear() => PlayerPrefs.DeleteKey(_playerPrefsKey);

        private static UnitSnapshot ToSnapshot(UI.UnitData unitData) => new()
        {
            Name = unitData.Name,
            MaxHealth = unitData.MaxHealth,
            CurrentHealth = unitData.CurrentHealth,
            Damage = unitData.Damage,
            Armor = unitData.Armor,
        };
    }
}

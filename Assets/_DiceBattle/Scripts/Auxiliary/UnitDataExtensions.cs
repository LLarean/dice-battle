using System.Collections.Generic;
using System.Linq;
using DiceBattle.Data;
using UnityEngine;

namespace DiceBattle
{
    public static class UnitDataExtensions
    {
        public static void Log(this UnitData unitData)
        {
#if UNITY_EDITOR
            string json = JsonUtility.ToJson(unitData);
            Debug.Log("---UnitData---");
            Debug.Log(json);
            Debug.Log("---End---");
#endif
        }

        public static void Update(this UnitData unitData, GameConfig config)
        {
            List<RewardType> rewardTypes = GameProgress.GetRewards().RewardTypes;

            unitData.Title = "Герой (upd)"; // TODO Translation

            unitData.Armor = rewardTypes.Count(r => r == RewardType.Armor) * config.PlayerBonusArmor;
            unitData.Attack = rewardTypes.Count(r => r == RewardType.AdditionalDamage) * config.PlayerBonusDamage;

            int doubleHealth = rewardTypes.Count(r => r == RewardType.DoubleHealth) * 2;

            if (doubleHealth > 0)
            {
                unitData.MaxHealth *= doubleHealth;
                unitData.CurrentHealth *= doubleHealth;
            }
        }
    }
}

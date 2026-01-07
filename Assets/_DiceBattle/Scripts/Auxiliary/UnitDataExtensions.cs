using System.Collections.Generic;
using System.Linq;
using DiceBattle.Data;
using DiceBattle.Global;
using DiceBattle.UI;
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
            List<RewardType> rewardTypes = GameProgress.GetReceivedRewards().RewardTypes;

            unitData.Title = "Герой (upd)"; // TODO Translation

            unitData.Armor = rewardTypes.Count(r => r == RewardType.Armor) * config.Player.GrowthArmor;
            unitData.Damage = rewardTypes.Count(r => r == RewardType.AdditionalDamage) * config.Player.GrowthDamage;

            int doubleHealth = rewardTypes.Count(r => r == RewardType.DoubleHealth) * 2;

            if (doubleHealth > 0)
            {
                unitData.MaxHealth *= doubleHealth;
                unitData.CurrentHealth *= doubleHealth;
            }
        }
    }
}

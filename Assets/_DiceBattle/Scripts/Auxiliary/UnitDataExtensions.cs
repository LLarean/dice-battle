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
            Debug.Log("<color=yellow>UnitData:</color> " + json);
#endif
        }

        public static void Update(this UnitData unitData, GameConfig config)
        {
            List<DiceType> rewardTypes = GameProgress.GetReceivedRewards().DiceTypes;

            unitData.Title = "Герой (upd)"; // TODO Translation

            unitData.Armor = rewardTypes.Count(r => r == DiceType.BaseArmor) * config.Player.GrowthArmor;
            unitData.Damage = rewardTypes.Count(r => r == DiceType.BaseDamage) * config.Player.GrowthDamage;

            int doubleHealth = rewardTypes.Count(r => r == DiceType.DoubleHealth) * 2;

            if (doubleHealth > 0)
            {
                unitData.MaxHealth *= doubleHealth;
                unitData.CurrentHealth *= doubleHealth;
            }
        }
    }
}

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
            Debug.Log("---UnitData---");
            Debug.Log("Title = " + unitData.Title );
            Debug.Log("Portrait.name = " + unitData.Portrait.name);
            Debug.Log("MaxHealth = " + unitData.MaxHealth);
            Debug.Log("CurrentHealth = " + unitData.CurrentHealth);
            Debug.Log("Attack = " + unitData.Attack);
            Debug.Log("Defense = " + unitData.Armor);
            Debug.Log("---End---");
        }

        public static void Update(this UnitData unitData, GameConfig config)
        {
            List<RewardType> rewardTypes = GameProgress.GetRewards().RewardTypes;

            unitData.Title = "Герой (upd)"; // TODO Translation

            unitData.Armor = rewardTypes.Count(r => r == RewardType.Armor) * config.ArmorBonus;
            unitData.Attack = rewardTypes.Count(r => r == RewardType.AdditionalDamage) * config.AttackBonus;

            // int doubleHealth = rewardTypes.Count(r => r == RewardType.DoubleHealth) * 2;
            //
            // if (doubleHealth > 0)
            // {
            //     _playerData.MaxHealth *= doubleHealth;
            //     _playerData.CurrentHealth *= doubleHealth;
            // }
        }
    }
}

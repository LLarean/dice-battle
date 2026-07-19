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
        public static UnitData CloneAtFullHealth(this UnitData source) => new()
        {
            Name = source.Name,
            Description = source.Description,
            Portrait = source.Portrait,
            Background = source.Background,
            MaxHealth = source.MaxHealth,
            CurrentHealth = source.MaxHealth,
            Damage = source.Damage,
            Armor = source.Armor,
        };

        public static void Log(this UnitData unitData)
        {
#if UNITY_EDITOR
            string json = JsonUtility.ToJson(unitData);
            Debug.Log("<color=yellow>UnitData:</color> " + json);
#endif
        }

        public static void Update(this UnitData unitData, GameConfig config)
        {
            UnitConfig playerConfig = config.GetPlayerConfig(GameData.SelectedCharacterClass);
            List<DiceType> rewardTypes = GameData.GetEquippedAsDiceList().DiceTypes;

            unitData.Name = "Герой (upd)"; // TODO Translation

            unitData.Armor = playerConfig.StartArmor + rewardTypes.Count(r => r == DiceType.BaseArmor) * playerConfig.GrowthArmor;
            unitData.Damage = playerConfig.StartDamage + rewardTypes.Count(r => r == DiceType.BaseDamage) * playerConfig.GrowthDamage;

            int doubleHealthCount = rewardTypes.Count(r => r == DiceType.BaseHealth);
            int newMaxHealth = playerConfig.StartHealth + playerConfig.StartHealth * doubleHealthCount;
            int maxHealthGain = newMaxHealth - unitData.MaxHealth;

            unitData.MaxHealth = newMaxHealth;
            unitData.CurrentHealth = Mathf.Clamp(unitData.CurrentHealth + maxHealthGain, 0, newMaxHealth);
        }
    }
}

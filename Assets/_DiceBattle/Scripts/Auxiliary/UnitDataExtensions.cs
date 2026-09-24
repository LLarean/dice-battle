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

            unitData.Name = playerConfig.Name;
            unitData.Armor = playerConfig.StartArmor;
            unitData.Damage = playerConfig.StartDamage;
        }
    }
}

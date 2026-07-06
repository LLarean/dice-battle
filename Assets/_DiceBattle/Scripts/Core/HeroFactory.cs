using System.Linq;
using DiceBattle.Data;
using DiceBattle.Global;
using DiceBattle.UI;

namespace DiceBattle.Core
{
    public static class HeroFactory
    {
        public static UnitData Build(UnitConfig unitConfig)
        {
            DiceList diceList = GameData.GetEquippedAsDiceList();

            int doubleHealthCount = diceList.DiceTypes.Count(r => r == DiceType.BaseHealth);
            int additionalHealth = unitConfig.StartHealth * doubleHealthCount;
            int maxHealth = unitConfig.StartHealth + additionalHealth;

            int baseDamageCount = diceList.DiceTypes.Count(r => r == DiceType.BaseDamage);
            int damage = unitConfig.StartDamage + baseDamageCount * unitConfig.GrowthDamage;

            int baseArmorCount = diceList.DiceTypes.Count(r => r == DiceType.BaseArmor);
            int armor = unitConfig.StartArmor + baseArmorCount * unitConfig.GrowthArmor;

            return new UnitData
            {
                MaxHealth = maxHealth,
                CurrentHealth = maxHealth,
                Damage = damage,
                Armor = armor,
            };
        }
    }
}

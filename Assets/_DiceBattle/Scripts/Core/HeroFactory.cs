using DiceBattle.Data;
using DiceBattle.UI;

namespace DiceBattle.Core
{
    public static class HeroFactory
    {
        public static UnitData Build(UnitConfig unitConfig)
        {
            return new UnitData
            {
                MaxHealth = unitConfig.StartHealth,
                CurrentHealth = unitConfig.StartHealth,
                Damage = unitConfig.StartDamage,
                Armor = unitConfig.StartArmor,
            };
        }
    }
}

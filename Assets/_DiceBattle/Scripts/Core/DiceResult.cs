using System.Collections.Generic;
using System.Linq;
using DiceBattle.UI;

namespace DiceBattle.Core
{
    public class DiceResult
    {
        private int _damage;
        private int _armor;
        private int _heal;

        public int Damage => _damage;
        public int Armor => _armor;
        public int Heal => _heal;

        public void Calculate(List<Dice> dices, DiceList equippedItems)
        {
            _damage = 0;
            _armor = 0;
            _heal = 0;

            foreach (Dice dice in dices)
            {
                switch (dice.DiceValue)
                {
                    case DiceValue.Attack:
                        _damage += CalculateSingle(DiceValue.Attack, equippedItems);
                        break;
                    case DiceValue.Defense:
                        _armor += CalculateSingle(DiceValue.Defense, equippedItems);
                        break;
                    case DiceValue.Heal:
                        _heal += CalculateSingle(DiceValue.Heal, equippedItems);
                        break;
                }
            }
        }

        public static int CalculateSingle(DiceValue diceValue, DiceList equippedItems)
        {
            int silverCount = equippedItems.DiceTypes.Count(t => t == DiceType.SilverDice);
            int goldCount = equippedItems.DiceTypes.Count(t => t == DiceType.GoldDice);
            int globalMultiplier = 1 + silverCount + goldCount * 2;

            int upgradeCount = diceValue switch
            {
                DiceValue.Attack => equippedItems.DiceTypes.Count(t => t == DiceType.UpgradeAttack),
                DiceValue.Defense => equippedItems.DiceTypes.Count(t => t == DiceType.UpgradeArmor),
                DiceValue.Heal => equippedItems.DiceTypes.Count(t => t == DiceType.UpgradeHealth),
                _ => 0,
            };

            return (1 + upgradeCount) * globalMultiplier;
        }
    }
}

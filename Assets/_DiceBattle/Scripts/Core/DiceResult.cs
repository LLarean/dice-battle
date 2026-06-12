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

            int silverCount = equippedItems.DiceTypes.Count(t => t == DiceType.SilverDice);
            int goldCount = equippedItems.DiceTypes.Count(t => t == DiceType.GoldDice);
            int globalMultiplier = 1 + silverCount + goldCount * 2;

            int upgradeAttack = equippedItems.DiceTypes.Count(t => t == DiceType.UpgradeAttack);
            int upgradeArmor = equippedItems.DiceTypes.Count(t => t == DiceType.UpgradeArmor);
            int upgradeHealth = equippedItems.DiceTypes.Count(t => t == DiceType.UpgradeHealth);

            foreach (Dice dice in dices)
            {
                switch (dice.DiceValue)
                {
                    case DiceValue.Attack:
                        _damage += (1 + upgradeAttack) * globalMultiplier;
                        break;
                    case DiceValue.Defense:
                        _armor += (1 + upgradeArmor) * globalMultiplier;
                        break;
                    case DiceValue.Heal:
                        _heal += (1 + upgradeHealth) * globalMultiplier;
                        break;
                }
            }
        }
    }
}

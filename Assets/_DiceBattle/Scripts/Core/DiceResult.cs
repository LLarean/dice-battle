using System.Collections.Generic;

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

        public void Calculate(List<Dice> dices)
        {
            _damage = 0;
            _armor = 0;
            _heal = 0;

            foreach (Dice dice in dices)
            {
                switch (dice.DiceValue)
                {
                    case DiceValue.Attack:
                        // if (dice.gameObject.activeSelf)
                        // {
                            _damage++;
                        // }
                        break;
                    case DiceValue.Defense:
                        _armor++;
                        break;
                    case DiceValue.Heal:
                        _heal++;
                        break;
                }
            }
        }
    }
}

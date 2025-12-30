using System.Collections.Generic;

namespace DiceBattle.Core
{
    public class TurnResult
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
                switch (dice.DiceType)
                {
                    case DiceType.Attack:
                        // if (dice.gameObject.activeSelf)
                        // {
                            _damage++;
                        // }
                        break;
                    case DiceType.Defense:
                        _armor++;
                        break;
                    case DiceType.Heal:
                        _heal++;
                        break;
                }
            }
        }
    }
}

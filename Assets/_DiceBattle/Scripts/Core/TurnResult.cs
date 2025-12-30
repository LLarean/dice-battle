using System.Collections.Generic;

namespace DiceBattle.Core
{
    public class TurnResult
    {
        private int _attack;
        private int _defense;
        private int _heal;

        public int Attack => _attack;
        public int Defense => _defense;
        public int Heal => _heal;

        public void Calculate(List<Dice> dices)
        {
            _attack = 0;
            _defense = 0;
            _heal = 0;

            foreach (Dice dice in dices)
            {
                switch (dice.DiceType)
                {
                    case DiceType.Attack:
                        if (dice.gameObject.activeSelf)
                        {
                            _attack++;
                        }
                        break;
                    case DiceType.Defense:
                        _defense++;
                        break;
                    case DiceType.Heal:
                        _heal++;
                        break;
                }
            }
        }
    }
}

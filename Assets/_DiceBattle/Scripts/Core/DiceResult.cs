using System.Collections.Generic;
using System.Linq;

namespace DiceBattle.Core
{
    public class DiceResult
    {
        private int _damage;
        private int _armor;
        private int _heal;
        private bool _isCritical;

        public const int CriticalAttackCount = 5;

        public int Damage => _damage;
        public int Armor => _armor;
        public int Heal => _heal;
        public bool IsCritical => _isCritical;

        public void Calculate(List<Dice> dices)
        {
            _damage = 0;
            _armor = 0;
            _heal = 0;
            _isCritical = dices.Count(dice => ResolveFace(dice, dices) == DiceValue.Attack) >= CriticalAttackCount;

            foreach (Dice dice in dices)
            {
                DiceValue face = ResolveFace(dice, dices);
                int value = FaceValue(dice, face, dices);

                switch (face)
                {
                    case DiceValue.Attack:
                        _damage += value;
                        _heal += dice.Type == DiceType.Vampiric ? 1 : 0;
                        break;
                    case DiceValue.Defense:
                        _armor += value;
                        _damage += dice.Type == DiceType.Thorns ? 1 : 0;
                        break;
                    case DiceValue.Heal:
                        _heal += value;
                        break;
                }
            }
        }

        /// <summary>
        /// Value of the die's main face, including the Golden bonus from the other dice.
        /// </summary>
        public static int FaceValue(Dice dice, List<Dice> dices)
        {
            DiceValue face = ResolveFace(dice, dices);
            return face == DiceValue.Empty ? 0 : FaceValue(dice, face, dices);
        }

        public static int OwnFaceValue(DiceType diceType, DiceValue face) =>
            1 + (diceType.GetEffectDiceValue() == face ? 1 : 0);

        private static int FaceValue(Dice dice, DiceValue face, List<Dice> dices)
        {
            int goldenBonus = dices.Count(other => other.Type == DiceType.Golden && ResolveFace(other, dices) == face);
            return OwnFaceValue(dice.Type, face) + goldenBonus;
        }

        public static DiceValue ResolveFace(Dice dice, List<Dice> dices)
        {
            if (dice.Type != DiceType.Joker || dice.DiceValue != DiceValue.Empty)
            {
                return dice.DiceValue;
            }

            // Ties go to the lower enum value, so Attack wins and helps toward a critical hit.
            return dices
                .Where(other => other != dice && other.DiceValue != DiceValue.Empty)
                .GroupBy(other => other.DiceValue)
                .OrderByDescending(group => group.Count())
                .ThenBy(group => group.Key)
                .Select(group => group.Key)
                .DefaultIfEmpty(DiceValue.Empty)
                .First();
        }
    }
}

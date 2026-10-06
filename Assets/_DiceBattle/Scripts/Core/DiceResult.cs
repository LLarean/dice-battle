using System.Collections.Generic;
using System.Linq;

namespace DiceBattle.Core
{
    public readonly struct DiceContribution
    {
        public readonly int Armor;
        public readonly int Damage;
        public readonly int Heal;

        public DiceContribution(int armor, int damage, int heal)
        {
            Armor = armor;
            Damage = damage;
            Heal = heal;
        }
    }

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
                DiceContribution contribution = Contribution(dice, dices);
                _damage += contribution.Damage;
                _armor += contribution.Armor;
                _heal += contribution.Heal;
            }
        }

        public static DiceContribution Contribution(Dice dice, List<Dice> dices)
        {
            DiceValue face = ResolveFace(dice, dices);
            int value = OwnFaceValue(dice.Type, face);

            return face switch {
                DiceValue.Attack => new DiceContribution(0, value, dice.Type == DiceType.Vampiric ? 1 : 0),
                DiceValue.Defense => new DiceContribution(value, dice.Type == DiceType.Thorns ? 1 : 0, 0),
                DiceValue.Heal => new DiceContribution(0, 0, value),
                _ => default,
            };
        }

        public static int FaceValue(Dice dice, List<Dice> dices) => OwnFaceValue(dice.Type, ResolveFace(dice, dices));

        public static int OwnFaceValue(DiceType diceType, DiceValue face)
        {
            if (face == DiceValue.Empty)
            {
                return 0;
            }

            bool isDoubled = diceType == DiceType.Golden || diceType.GetEffectDiceValue() == face;
            return isDoubled ? 2 : 1;
        }

        public static DiceValue ResolveFace(Dice dice, List<Dice> dices)
        {
            if (dice.Type != DiceType.Joker)
            {
                return dice.DiceValue;
            }

            // GroupBy keeps first-seen order and the sort is stable, so a tie goes to the leftmost face.
            // With nothing to copy the Joker keeps its own roll.
            return dices
                .Where(other => other != dice && other.DiceValue != DiceValue.Empty)
                .GroupBy(other => other.DiceValue)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key)
                .DefaultIfEmpty(dice.DiceValue)
                .First();
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using DiceBattle.Global;
using DiceBattle.UI;

namespace DiceBattle.Core
{
    /// <summary>
    /// Active deck context. Campaign uses the equipped inventory (default);
    /// tournament switches to a standard deck with no inventory multipliers.
    /// </summary>
    public static class DiceRuleset
    {
        private static DiceList _override;
        private static int _diceCountOverride;

        public static DiceList Current => _override ?? GameData.GetEquippedAsDiceList();

        public static int DiceCount(int baseCount) =>
            _override != null
                ? _diceCountOverride
                : baseCount + Current.DiceTypes.Count(t => t == DiceType.AdditionalDice);

        /// <summary>
        /// Die types in board order: equipped dice first, free slots filled with default dice.
        /// </summary>
        public static List<DiceType> Deck(int baseCount)
        {
            int count = DiceCount(baseCount);

            return Current.DiceTypes
                .Where(type => type != DiceType.AdditionalDice)
                .Concat(Enumerable.Repeat(DiceType.Default, count))
                .Take(count)
                .ToList();
        }

        public static void SetStandard(int diceCount)
        {
            _override = new DiceList();
            _diceCountOverride = diceCount;
        }

        public static void Reset() => _override = null;
    }
}
